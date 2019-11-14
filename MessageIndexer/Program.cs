using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
using MessageIndexer.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nest;

namespace MessageIndexer
{
    public class Program
    {
        public static async Task Main(string[] args) => await CreateHostBuilder(args).Build().RunAsync();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddSingleton<IElasticClient>(Create());
                });

        private static IElasticClient Create()
        {
            var connectionSettings = new ConnectionSettings(new SingleNodeConnectionPool(new Uri("http://localhost:9200")))
                    .DefaultMappingFor<MessageIndexItem>(i => i
                    .IndexName("messages"))
                .PrettyJson()
                .RequestTimeout(TimeSpan.FromMinutes(2));

            return new ElasticClient(connectionSettings);
        }

    }
}
