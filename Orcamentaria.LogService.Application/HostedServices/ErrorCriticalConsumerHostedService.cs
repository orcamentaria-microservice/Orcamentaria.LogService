using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Orcamentaria.Lib.Domain.Exceptions;
using Orcamentaria.Lib.Domain.Models.Configurations;
using Orcamentaria.Lib.Domain.Services;

namespace Orcamentaria.LogService.Application.HostedServices
{
    public class ErrorCriticalConsumerHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly string _queue;

        public ErrorCriticalConsumerHostedService(
            IOptions<MessageBrokerConfiguration> options,
            IServiceScopeFactory scopeFactory)
        {
            _queue = "error.critical";
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var consumer = scope.ServiceProvider.GetRequiredService<IMessageBrokerConsumerService>();
                var processor = scope.ServiceProvider.GetRequiredKeyedService<IMessageBrokerProcessorService>(_queue);

                await consumer.HandleBasicDeliverAsync(
                    queueConsume: _queue,
                    stoppingToken: stoppingToken,
                    processMessage: processor.ProcessAsync);

            }
            catch (DefaultException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new UnexpectedException(ex.Message, ex);
            }
        }
    }
}
