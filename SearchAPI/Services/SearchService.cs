using Grpc.Core;
using Microsoft.Extensions.Logging;
using SearchAPI.Elastic;
using System;
using System.Threading.Tasks;

namespace SearchAPI.Services
{
    public class SearchService : SearchAPI.SearchAPIBase
    {
        public ILogger<SearchService> Logger { get; }
        public ISearchClient Client { get; }

        public SearchService(ILogger<SearchService> logger, ISearchClient client)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public override async Task<SearchResponse> Search(SearchRequest request, ServerCallContext context)
        {
            Logger.LogInformation("Searching for {query}", request.Query);

            var hits = await Client.Search(request.Query, 0);

            var response = new SearchResponse();

            foreach(var hit in hits)
            {
                response.Results.Add(
                    new Result() 
                    { 
                        Id = hit.Id, 
                        DisplayName = hit.DisplayName, 
                        Message = hit.Text, 
                        Channel = hit.Channel, 
                        Timestamp = hit.TimeStamp 
                    });
            }

            return response;
        }

        public override async Task<ScrollResponse> InitScroll(InitScrollRequest request, ServerCallContext context)
        {
            Logger.LogInformation("Initializing scroll for {Timestamp}", request.Timestamp);

            var result = await Client.InitScroll(request.Size, request.Timestamp, request.Direction);

            var response = new ScrollResponse() { ScrollId = result.ScrollId };

            foreach (var message in result.Messages)
            {
                response.Results.Add(
                    new Result()
                    {
                        Id = message.Id,
                        DisplayName = message.DisplayName,
                        Message = message.Text,
                        Channel = message.Channel,
                        Timestamp = message.TimeStamp
                    });
            }

            return response;
        }


        public override async Task<ScrollResponse> Scroll(ScrollRequest request, ServerCallContext context)
        {
            Logger.LogInformation("Received scroll request for {id}", request.ScrollId);

            var result = await Client.Scroll(request.ScrollId);

            var response = new ScrollResponse() { ScrollId = result.ScrollId };

            foreach (var message in result.Messages)
            {
                response.Results.Add(
                    new Result()
                    {
                        Id = message.Id,
                        DisplayName = message.DisplayName,
                        Message = message.Text,
                        Channel = message.Channel,
                        Timestamp = message.TimeStamp
                    });
            }

            return response;
        }
    }
}
