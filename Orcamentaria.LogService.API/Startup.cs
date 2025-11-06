using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using Orcamentaria.Lib.Domain.Models.Configurations;
using Orcamentaria.Lib.Domain.Services;
using Orcamentaria.Lib.Infrastructure;
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
            Configuration = CommonDI.ResolveConfigs(_serviceName, services, Configuration);
            services.Replace(ServiceDescriptor.Singleton(Configuration));

            CommonDI.AddServiceRegistryHosted(services, Configuration);

            CommonDI.ResolveCommonServices(_serviceName, _apiVersion, services, Configuration, () =>
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
            => CommonDI.ConfigureCommon(_serviceName, _apiVersion, app, env);

    }
}
