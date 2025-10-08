using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Orcamentaria.Lib.Domain.Exceptions;
using Orcamentaria.Lib.Domain.Models.Configurations;
using Orcamentaria.LogService.Domain.Services;
using RabbitMQ.Client;

namespace Orcamentaria.LogService.Application.HostedServices
{
    public class ErrorConsumerHostedService : BackgroundService
    {
        private readonly MessageBrokerConfiguration _messageBrokerConfiguration;
        private readonly IServiceScopeFactory _scopeFactory;

        private IConnection? _connection;
        private IChannel? _channel;

        public ErrorConsumerHostedService(
            IOptions<MessageBrokerConfiguration> options,
            IServiceScopeFactory scopeFactory)
        {
            if (string.IsNullOrWhiteSpace(options.Value.ErrorQueue))
                throw new ConfigurationException("Informe a queue do RabbitMq a ser consumida.");

            _messageBrokerConfiguration = options.Value;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var consumer = scope.ServiceProvider.GetRequiredService<IMessageBrokerConsumerService>();
                var processor = scope.ServiceProvider.GetRequiredKeyedService<IMessageBrokerProcessorService>(_messageBrokerConfiguration.ErrorQueue);

                await consumer.HandleBasicDeliverAsync(_messageBrokerConfiguration.ErrorQueue, stoppingToken, processor.ProcessAsync);
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

        public override async Task StopAsync(CancellationToken cancellationToken)
            => await base.StopAsync(cancellationToken);

        public override void Dispose()
        {
            try { _channel?.Dispose(); } catch { }
            try { _connection?.Dispose(); } catch { }
            base.Dispose();
        }
    }
}
