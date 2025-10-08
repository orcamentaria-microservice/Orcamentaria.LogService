using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Orcamentaria.Lib.Domain.Models.Configurations;
using Orcamentaria.Lib.Infrastructure;
using Orcamentaria.LogService.Application.HostedServices;
using Orcamentaria.LogService.Application.Services;
using Orcamentaria.LogService.Domain.Repositories;
using Orcamentaria.LogService.Domain.Services;
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

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            CommonDI.AddServiceRegistryHosted(services, Configuration);

            CommonDI.ResolveCommonServices(_serviceName, _apiVersion, services, Configuration, () =>
            {
                services.Configure<MessageBrokerConfiguration>(Configuration.GetSection("MessageBroker"));

                services.AddScoped<IMongoClient>(_ => new MongoClient(Configuration.GetConnectionString("DefaultConnection")));

                services.AddScoped<IExceptionLogRepository, ExceptionLogRepository>();

                services.AddScoped<IMessageBrokerConsumerService, RabbitMqConsumeService>();

                var messageBrokerConfig = Configuration.GetSection("MessageBroker").Get<MessageBrokerConfiguration>();

                services.AddKeyedScoped<IMessageBrokerProcessorService, ErrorMessageProcessorService>(messageBrokerConfig?.ErrorQueue);

                services.AddHostedService<ErrorConsumerHostedService>();

                services.AddScoped<MongoContext>();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            => CommonDI.ConfigureCommon(_serviceName, _apiVersion, app, env);

    }
}
