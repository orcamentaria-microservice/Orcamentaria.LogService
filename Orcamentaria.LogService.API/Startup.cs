using MongoDB.Driver;
using Orcamentaria.Lib.Infrastructure;
using Orcamentaria.LogService.Application.Services;
using Orcamentaria.LogService.Domain.HostedServices;
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

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            CommonDI.AddServiceRegistryHosted(services, Configuration);

            CommonDI.ResolveCommonServices(_serviceName, _apiVersion, services, Configuration, () =>
            {
                services.AddScoped<IMongoClient>(_ => new MongoClient(Configuration.GetConnectionString("DefaultConnection")));

                services.AddScoped<IExceptionLogRepository, ExceptionLogRepository>();

                services.AddScoped<IConsumerBrokerHostedService>(_ => new RabbitMqConsumeService(String.Empty));

                services.AddScoped<MongoContext>();
            });

            //services.AddHostedService<RabbitMqHostedService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            => CommonDI.ConfigureCommon(_serviceName, _apiVersion, app, env);

    }
}
