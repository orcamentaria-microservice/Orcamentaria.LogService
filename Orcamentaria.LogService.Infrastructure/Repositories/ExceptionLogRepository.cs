using MongoDB.Driver;
using Orcamentaria.Lib.Domain.Exceptions;
using Orcamentaria.Lib.Domain.Models.Logs;
using Orcamentaria.LogService.Domain.Repositories;
using Orcamentaria.LogService.Infrastructure.Contexts;

namespace Orcamentaria.LogService.Infrastructure.Repositories
{
    public class ExceptionLogRepository : IExceptionLogRepository
    {
        private readonly MongoContext _dbContext;

        public ExceptionLogRepository(MongoContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ExceptionLog>> GetExceptionLogs(DateTime startDate, DateTime endDate)
        {
            try
            {
                var data = await _dbContext.ExceptionLog.FindAsync(x => 
                    x.Date >= startDate && 
                    x.Date <= endDate);
                return data.ToList();
            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message);
            }
        }

        public async Task<IEnumerable<ExceptionLog>> GetExceptionLogsByCode(int code, DateTime startDate, DateTime endDate)
        {
            try
            {
                var data = await _dbContext.ExceptionLog.FindAsync(x => 
                    x.Code == code && 
                    x.Date >= startDate && 
                    x.Date <= endDate);
                return data.ToList();
            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message);
            }
        }

        public async Task<IEnumerable<ExceptionLog>> GetExceptionLogsByService(string serviceName, DateTime startDate, DateTime endDate)
        {
            try
            {
                var data = await _dbContext.ExceptionLog.FindAsync(x =>
                    x.Place.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase) &&
                    x.Date >= startDate &&
                    x.Date <= endDate);
                return data.ToList();
            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message);
            }
        }

        public async Task<IEnumerable<ExceptionLog>> GetExceptionLogsBySeverity(string severity, DateTime startDate, DateTime endDate)
        {
            try
            {
                var data = await _dbContext.ExceptionLog.FindAsync(x =>
                    x.Severity.Equals(severity, StringComparison.OrdinalIgnoreCase) &&
                    x.Date >= startDate &&
                    x.Date <= endDate);
                return data.ToList();
            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message);
            }
        }

        public async Task<IEnumerable<ExceptionLog>> GetExceptionLogsByType(string type, DateTime startDate, DateTime endDate)
        {
            try
            {
                var data = await _dbContext.ExceptionLog.FindAsync(x =>
                    x.Type.Equals(type, StringComparison.OrdinalIgnoreCase) &&
                    x.Date >= startDate &&
                    x.Date <= endDate);
                return data.ToList();
            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message);
            }
        }

        public async Task Insert(ExceptionLog exceptionLog)
        {
            try
            {
                await _dbContext.ExceptionLog.InsertOneAsync(exceptionLog);
            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message);
            }
        }
    }
}
