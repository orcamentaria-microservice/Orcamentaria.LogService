namespace Orcamentaria.LogService.Domain.Services
{
    public interface IMessageBrokerConsumerService
    {
        Task HandleBasicDeliverAsync(
            string queueConsume, 
            CancellationToken stoppingToken, 
            Func<string, Task> processMessage);
    }
}
