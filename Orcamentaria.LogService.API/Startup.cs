using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using Orcamentaria.Lib.Domain.Models.Configurations;
using Orcamentaria.Lib.Domain.Services;
using Orcamentaria.Lib.Infrastructure;
using Orcamentaria.Lib.Infrastructure.Configures;
using Orcamentaria.LogService.Application.HostedServices;
using Orcamentaria.LogService.Application.Services;
using Orcamentaria.LogService.Domain.Repositories;
using Orcamentaria.LogService.Infrastructure.Contexts;
using Orcamentaria.LogService.Infrastructure.Repositories;

namespace Orcamentaria.LogService.API
{
    public class Startup
    {
        private readonly string _serviceName = "Orcamentaria.LogService";
        private readonly string _apiVersion = "v1";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            Configuration = services.ResolveConfigs(Configuration, _serviceName);

            services.Replace(ServiceDescriptor.Singleton(Configuration));

            services.AddServiceRegistryHosted(Configuration);

            services.ResolveCommonServices(
                configuration: Configuration,
                serviceName: _serviceName,
                apiVersion: _apiVersion,
                customServices: () =>
                {
                services.AddScoped<IMongoClient>(_ => new MongoClient(Configuration.GetConnectionString("DefaultConnection")));
                services.AddScoped<MongoContext>();
                
                services.Configure<MessageBrokerConfiguration>(Configuration.GetSection("MessageBrokerConfiguration"));

                services.AddScoped<IExceptionLogRepository, ExceptionLogRepository>();
                services.AddKeyedScoped<IMessageBrokerProcessorService, ErrorCriticalMessageProcessorService>("error.critical");
                services.AddKeyedScoped<IMessageBrokerProcessorService, ErrorInfoMessageProcessorService>("error.info");

                services.AddHostedService<ErrorCriticalConsumerHostedService>();
                services.AddHostedService<ErrorInfoConsumerHostedService>();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            => app.ConfigureCommon(env, _serviceName, _apiVersion);

    }
}
