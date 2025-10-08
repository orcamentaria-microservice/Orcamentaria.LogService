using Microsoft.Extensions.Options;
using Orcamentaria.Lib.Domain.Exceptions;
using Orcamentaria.Lib.Domain.Models.Configurations;
using Orcamentaria.LogService.Domain.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

public sealed class RabbitMqConsumeService : IMessageBrokerConsumerService
{
    private readonly MessageBrokerConfiguration _messageBrokerConfiguration;
    private readonly CancellationToken _stoppingToken;

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
        Func<string, Task<bool>> processMessage)
    {
        try 
        {
            var factory = new ConnectionFactory
            {
                HostName = _messageBrokerConfiguration.Host,
            };

            _connection = await factory.CreateConnectionAsync(_stoppingToken);
            _channel = await _connection.CreateChannelAsync();

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (_, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                if(await processMessage(message))
                    await _channel.BasicAckAsync(deliveryTag: eventArgs.DeliveryTag, multiple: false);
            };

            await _channel.BasicConsumeAsync(queue: queueConsume, autoAck: false, consumer: consumer);

            while (!_stoppingToken.IsCancellationRequested)
                await Task.Delay(1000, _stoppingToken);
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
