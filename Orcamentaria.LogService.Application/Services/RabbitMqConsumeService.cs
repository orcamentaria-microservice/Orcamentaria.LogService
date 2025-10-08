using Microsoft.Extensions.Options;
using Orcamentaria.Lib.Domain.Enums;
using Orcamentaria.Lib.Domain.Exceptions;
using Orcamentaria.Lib.Domain.Models.Configurations;
using Orcamentaria.Lib.Domain.Models.Exceptions;
using Orcamentaria.LogService.Domain.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

public sealed class RabbitMqConsumeService : IMessageBrokerConsumerService
{
    private readonly MessageBrokerConfiguration _messageBrokerConfiguration;

    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqConsumeService(IOptions<MessageBrokerConfiguration> options)
    {
        if (string.IsNullOrWhiteSpace(options.Value.Host))
            throw new ConfigurationException("Informe o host do RabbitMq.");

        _messageBrokerConfiguration = options.Value;
    }

    public async Task HandleBasicDeliverAsync(
        string queueConsume,
        CancellationToken stoppingToken, 
        Func<string, Task> processMessage)
    {
        try 
        {
            var factory = new ConnectionFactory
            {
                HostName = _messageBrokerConfiguration.Host,
            };

            _connection = await factory.CreateConnectionAsync(stoppingToken);
            _channel = await _connection.CreateChannelAsync();

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (_, eventArgs) =>
            {
                try
                {
                    var body = eventArgs.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    await processMessage(message);
                    await _channel.BasicAckAsync(deliveryTag: eventArgs.DeliveryTag, multiple: false);
                }
                catch (DefaultException)
                {
                    await _channel.BasicNackAsync(deliveryTag: eventArgs.DeliveryTag, multiple: false, requeue: true);
                    throw;
                }
                catch (Exception ex)
                {
                    await _channel.BasicNackAsync(deliveryTag: eventArgs.DeliveryTag, multiple: false, requeue: true);
                    throw new UnexpectedException(ex.Message, ex);
                }
            };

            await _channel.BasicConsumeAsync(queue: queueConsume, autoAck: false, consumer: consumer);

            while (!stoppingToken.IsCancellationRequested)
                await Task.Delay(1000, stoppingToken);
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
