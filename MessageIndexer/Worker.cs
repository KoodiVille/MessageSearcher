using MessageIndexer.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MessageIndexer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private IEnumerable<string> _messageFiles;
        private readonly string _root;
        private readonly IElasticClient _client;

        public Worker(ILogger<Worker> logger, IHostEnvironment hostEnvironment, IElasticClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _root = hostEnvironment.ContentRootPath ?? throw new ArgumentNullException(nameof(hostEnvironment));
            _messageFiles = new DirectoryInfo($"{hostEnvironment.ContentRootPath}{Path.DirectorySeparatorChar}TestData{Path.DirectorySeparatorChar}").GetDirectories("*").SelectMany(x => Directory.EnumerateFiles(x.FullName));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var users = await GetUsersAsync($"{_root}{Path.DirectorySeparatorChar}TestData{Path.DirectorySeparatorChar}users.json");

            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            var tasks = _messageFiles.AsParallel().Select(x => GetMessagesAsync(x, GetChannelName(x)));
            var messages = new List<ChannelMessages>();

            foreach (var task in tasks)
            {
                messages.Add(await task);
            }

            _logger.LogInformation("{count}", messages.Count);

            var outer = messages.AsParallel().Select(messages => messages.Messages.AsParallel().Select(message =>
            {
                var userId = message.User ?? string.Empty;
                users.TryGetValue(userId, out var user);
                var id = message.TimeStamp + DateTime.Now.Ticks.ToString();
                return new MessageIndexItem(id.ToString(), user?.Name, user?.Profile?.DisplayName, messages.Channel, FromUnixTime(message.TimeStamp), message.Text);
            }));

            var indexItems = new List<MessageIndexItem>();

            foreach (var outerTask in outer)
            {
                foreach (var innerTask in outerTask)
                {
                    indexItems.Add(innerTask);
                }
            }

            _logger.LogInformation("Collisions {count}", indexItems.NonDistinct(x => x.Id).ToList().Count);

            var observable =
                _client.BulkAll(
                    indexItems,
                    b => b.Index("messages")
                    .BackOffTime("30s")
                    .BackOffRetries(2)
                    .RefreshOnCompleted()
                    .MaxDegreeOfParallelism(Environment.ProcessorCount)
                    .Size(1000))
                .Wait(TimeSpan.FromMinutes(15), next =>
                {
                    _logger.LogInformation("{page}", next.Page);
                });

            _logger.LogInformation("Total number of failures: {failures}", observable.TotalNumberOfFailedBuffers);
        }


        private JsonSerializerOptions GetOptions()
        {
            return new JsonSerializerOptions { PropertyNameCaseInsensitive = true, IgnoreNullValues = true };
        }

        private async ValueTask<Dictionary<string, User>> GetUsersAsync(string path)
        {
            _logger.LogInformation("Reading users from file: {path}", path);

            using var fs = File.OpenRead(path);
            var users = await JsonSerializer.DeserializeAsync<IEnumerable<User>>(fs, GetOptions());
            var dict = new Dictionary<string, User>();

            foreach (var user in users)
            {
                dict.Add(user.Id, user);
            }

            return dict;
        }

        private async ValueTask<ChannelMessages> GetMessagesAsync(string path, string channel)
        {
            _logger.LogInformation("Reading messages from file: {path}", path);

            using var fs = File.OpenRead(path);
            var messages = await JsonSerializer.DeserializeAsync<IEnumerable<Message>>(fs, GetOptions());

            return new ChannelMessages(channel, messages);
        }

        private string GetChannelName(string path)
        {
            var dir = Path.GetDirectoryName(path);
            var dirs = dir.Split(Path.DirectorySeparatorChar);

            return dirs.Last();
        }

        private DateTime FromUnixTime(string ts)
        {
            var time = ts.Split('.')[0];
            if (long.TryParse(time, out var result))
            {
                return DateTimeOffset.FromUnixTimeSeconds(result).DateTime;
            }
            return DateTime.MinValue;
        }

    }

    public static class Extensions
    {
        public static IEnumerable<T> NonDistinct<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        {
            return source.GroupBy(keySelector).Where(g => g.Count() > 1).SelectMany(r => r);
        }
    }
}