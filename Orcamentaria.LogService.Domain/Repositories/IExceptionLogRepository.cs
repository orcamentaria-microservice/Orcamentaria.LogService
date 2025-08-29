using Orcamentaria.Lib.Domain.Models.Logs;

namespace Orcamentaria.LogService.Domain.Repositories
{
    public interface IExceptionLogRepository
    {
        Task<IEnumerable<ExceptionLog>> GetExceptionLogsByService(string serviceName, DateTime startDate, DateTime endDate);
        Task<IEnumerable<ExceptionLog>> GetExceptionLogsByType(string type, DateTime startDate, DateTime endDate);
        Task<IEnumerable<ExceptionLog>> GetExceptionLogsBySeverity(string severity, DateTime startDate, DateTime endDate);
        Task<IEnumerable<ExceptionLog>> GetExceptionLogsByCode(int code, DateTime startDate, DateTime endDate);
        Task<IEnumerable<ExceptionLog>> GetExceptionLogs(DateTime startDate, DateTime endDate);
        Task Insert(ExceptionLog exceptionLog);
    }
}
