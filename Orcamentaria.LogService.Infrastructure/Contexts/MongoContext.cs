using MongoDB.Driver;
using Orcamentaria.Lib.Domain.Models.Logs;

namespace Orcamentaria.LogService.Infrastructure.Contexts
{
    public class MongoContext
    {
        private static readonly string _databaseName = "log-db";
        public IMongoCollection<ExceptionLog> ExceptionLog { get; private set; }
        public MongoContext(IMongoClient client)
        {
            ExceptionLog = 
                client.GetDatabase(_databaseName).GetCollection<ExceptionLog>("exception-logs");
        }

    }
}
