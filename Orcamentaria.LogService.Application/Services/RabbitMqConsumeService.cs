using Orcamentaria.Lib.Domain.Exceptions;
using Orcamentaria.LogService.Domain.HostedServices;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

namespace Orcamentaria.LogService.Application.Services
{
    public class RabbitMqConsumeService : IConsumerBrokerHostedService
    {
        private readonly IChannel _channel;
        public RabbitMqConsumeService(string host)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(host))
                    throw new ConfigurationException("Informe o host do RabbitMq.");

                var factory = new ConnectionFactory { HostName = host };
                var connection = factory.CreateConnectionAsync().Result;
                var _channel = connection.CreateChannelAsync().Result;
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

        public async Task HandleBasicDeliverAsync(string queueConsume)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                await _channel.BasicAckAsync(deliveryTag: eventArgs.DeliveryTag, multiple: false);
            };

            await _channel.BasicConsumeAsync(queue: queueConsume, autoAck: false, consumer: consumer);
        }
    }
}
