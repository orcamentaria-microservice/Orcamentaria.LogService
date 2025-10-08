namespace Orcamentaria.LogService.Domain.Services
{
    public interface IMessageBrokerProcessorService
    {
        Task ProcessAsync(string message);
    }
}
