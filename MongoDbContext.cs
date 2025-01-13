using MongoDB.Driver;

namespace OpenIdAuthServer;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration configuration)
    {
        var client = new MongoClient(configuration.GetConnectionString("MongoConnectionString"));
        _database = client.GetDatabase("OpenIdAuthServer");
    }

    public IMongoCollection<T> GetCollection<T>(string name) => _database.GetCollection<T>(name);
    public IMongoDatabase GetDatabase() => _database;
}