using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ECommerce_App.Services
{
    public class MyService
    {
        private readonly IMongoDatabase _database;

        public MyService(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }
    }

}
