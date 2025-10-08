using Orcamentaria.LogService.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orcamentaria.LogService.Application.Services
{
    public class ErrorMessageProcessorService : IMessageBrokerProcessorService
    {
        public async Task<bool> ProcessAsync(string message)
        {
            Console.WriteLine(message);

            return true;
        }
    }
}
