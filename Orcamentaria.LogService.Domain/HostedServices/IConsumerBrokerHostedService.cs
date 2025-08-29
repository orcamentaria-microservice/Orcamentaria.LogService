namespace Orcamentaria.LogService.Domain.HostedServices
{
    public interface IConsumerBrokerHostedService
    {
        Task HandleBasicDeliverAsync(string queueConsume);
    }
}
