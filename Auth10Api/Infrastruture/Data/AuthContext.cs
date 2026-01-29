using Auth10Api.Domain.Entities;
using MongoDB.Driver;

namespace Auth10Api.Infrastruture.Data;

public class AuthContext
{
    private readonly IMongoDatabase _database;

    public AuthContext(IConfiguration configuration)
    {
        var connectString = configuration["MongoDB:ConnectionString"];

        var client = new MongoClient(connectString);

        _database = client.GetDatabase(configuration["MongoDB:DatabaseName"]);
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("User");
}
