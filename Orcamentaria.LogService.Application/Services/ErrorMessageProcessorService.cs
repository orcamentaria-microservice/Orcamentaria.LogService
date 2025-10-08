using Orcamentaria.Lib.Domain.Enums;
using Orcamentaria.Lib.Domain.Exceptions;
using Orcamentaria.Lib.Domain.Models.Exceptions;
using Orcamentaria.Lib.Domain.Models.Logs;
using Orcamentaria.LogService.Domain.Repositories;
using Orcamentaria.LogService.Domain.Services;
using System;
using System.Text.Json;

namespace Orcamentaria.LogService.Application.Services
{
    public class ErrorMessageProcessorService : IMessageBrokerProcessorService
    {
        private readonly IExceptionLogRepository _exceptionLogRepository;

        public ErrorMessageProcessorService(IExceptionLogRepository exceptionLogRepository)
        {
            _exceptionLogRepository = exceptionLogRepository;
        }

        public async Task ProcessAsync(string message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(message))
                    throw new InfoException("Mensagem de erro inválida.", ErrorCodeEnum.ValidationFailed);

                var obj = JsonSerializer.Deserialize<ExceptionLog>(message);

                if (obj == null)
                    throw new InfoException("Falha ao desserializar a mensagem de erro.", ErrorCodeEnum.ValidationFailed);

                await _exceptionLogRepository.Insert(obj);
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
}
