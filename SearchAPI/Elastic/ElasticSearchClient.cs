using Elasticsearch.Net;
using Microsoft.Extensions.Logging;
using Nest;
using SearchAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SearchAPI.InitScrollRequest.Types;

namespace SearchAPI.Elastic
{
    public class ElasticSearchClient : ISearchClient
    {
        private readonly string scrollTimeout = "2m";
        public IElasticClient Client { get; }
        public ILogger<ElasticSearchClient> Logger { get; }

        public ElasticSearchClient(ILogger<ElasticSearchClient> logger, IElasticClient client)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Message>> Search(string query, int from, int size)
        {
            var response = await Client.SearchAsync<Message>(
                s => s
                .Size(size)
                .From(from)
                .Query(
                    q => q.Bool(
                        b => b.Should(
                            sh => sh.Match(m => m
                                .Field("text")
                                .Query(query))))));

            return response.Hits.Select(x => x.Source);
        }

        public async Task<ScrollResult> InitScroll(int size, string timestamp, Direction direction)
        {
            try
            {
                var response = await Client.SearchAsync<Message>(
                    s => s
                        .Size(size)
                        .Scroll(scrollTimeout)
                        .Query(
                            q => q.DateRange(r => r
                                .Field("timestamp")
                                .AddDateQueryDescriptor(direction, timestamp)))
                        .Sort(so => so.Field(
                            f => f.Field("timestamp").Ascending())));


                return new ScrollResult(response.Hits.Select(x => x.Source), response.ScrollId);
            }
            catch (ElasticsearchClientException e)
            {
                Logger.LogError(e, "unexpected error while initializin scroll for { timestamp }", timestamp);
                throw;
            }
        }


        public async Task<ScrollResult> Scroll(string scrollId)
        {
            var response = await Client.ScrollAsync<Message>(scrollTimeout, scrollId);

            return new ScrollResult(response.Hits.Select(x => x.Source), response.ScrollId);
        }
    }
    public static class DateRangeQueryDescriptorExtensions
    {
        public static DateRangeQueryDescriptor<T> AddDateQueryDescriptor<T>(this DateRangeQueryDescriptor<T> drqd, Direction direction, string timestamp) where T : class
        {
            switch (direction)
            {
                case Direction.Gte:
                    return drqd.GreaterThanOrEquals(timestamp);
                case Direction.Lte:
                    return drqd.LessThanOrEquals(timestamp);
                default:
                    return drqd;
            }
        }
    }
}

