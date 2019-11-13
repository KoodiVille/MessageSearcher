using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MessageIndexer.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MessageIndexer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private IEnumerable<string> _messageFiles;
        private readonly string _root;

        public Worker(ILogger<Worker> logger, IHostEnvironment hostEnvironment)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _root = hostEnvironment.ContentRootPath ?? throw new ArgumentNullException(nameof(hostEnvironment));
            _messageFiles = new DirectoryInfo($"{hostEnvironment.ContentRootPath}{Path.DirectorySeparatorChar}TestData{Path.DirectorySeparatorChar}").GetDirectories("*").SelectMany(x => Directory.EnumerateFiles(x.FullName));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var users = await GetUsersAsync($"{_root}{Path.DirectorySeparatorChar}TestData{Path.DirectorySeparatorChar}users.json");

            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            var result = new List<Message>();

            foreach (var file in _messageFiles)
            {
                var messages = await GetMessagesAsync(file, GetChannelName(file));
                result.AddRange(messages);
            }

            _logger.LogInformation("{count}", result.Count());

            await Task.Delay(1000, stoppingToken);
        }

        private JsonSerializerOptions GetOptions()
        {
            return new JsonSerializerOptions { PropertyNameCaseInsensitive = true, IgnoreNullValues = true };
        }

        private async ValueTask<IEnumerable<User>> GetUsersAsync(string path)
        {
            _logger.LogInformation("Reading users from file: {path}", path);

            using var fs = File.OpenRead(path);
            var users = await JsonSerializer.DeserializeAsync<IEnumerable<User>>(fs, GetOptions());

            return users;
        }

        private async ValueTask<IEnumerable<Message>> GetMessagesAsync(string path, string channel)
        {
            _logger.LogInformation("Reading messages from file: {path}", path);

            using var fs = File.OpenRead(path);
            var messages = await JsonSerializer.DeserializeAsync<IEnumerable<Message>>(fs, GetOptions());
            return messages;
        }

        private string GetChannelName(string path)
        {
            var dir = Path.GetDirectoryName(path);
            var dirs = dir.Split(Path.DirectorySeparatorChar);
            return dirs.Last();
        }
    }
}