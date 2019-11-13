using System;
using Elasticsearch.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nest;
using SearchAPI.Elastic;
using SearchAPI.Models;
using SearchAPI.Services;

namespace SearchAPI
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddSingleton<IElasticClient>(Create());
            services.AddTransient<ISearchClient, ElasticSearchClient>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<SearchService>();
            });
        }

        private IElasticClient Create()
        {
            var connectionSettings = new ConnectionSettings(new SingleNodeConnectionPool(new Uri("http://localhost:9200")))
                    .DefaultMappingFor<Message>(i => i
                    .IndexName("messages")
                    .TypeName("messages"))
                .PrettyJson()
                .RequestTimeout(TimeSpan.FromMinutes(2));

            return new ElasticClient(connectionSettings);
        }
    }
}
