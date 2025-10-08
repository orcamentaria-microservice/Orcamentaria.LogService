namespace Orcamentaria.LogService.Domain.Services
{
    public interface IMessageBrokerProcessorService
    {
        Task<bool> ProcessAsync(string message);
    }
}
