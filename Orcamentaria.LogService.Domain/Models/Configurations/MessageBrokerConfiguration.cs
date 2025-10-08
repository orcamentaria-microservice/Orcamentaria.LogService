namespace Orcamentaria.Lib.Domain.Models.Configurations
{
    public class MessageBrokerConfiguration
    {
        public string BrokerName { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string ErrorQueue { get; set; }
        public string ErrorCriticalQueue { get; set; }

    }
}
