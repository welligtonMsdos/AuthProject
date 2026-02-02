using Auth10Api.Domain.Entities;
using MongoDB.Driver;

namespace Auth10Api.Infrastruture.Data;

public class AuthContext
{
    private readonly IMongoDatabase _database;

    public IMongoCollection<User> Users => _database.GetCollection<User>("User");

    public AuthContext(IMongoClient client, IConfiguration config)
    {   
        var dbName = config.GetValue<string>("MongoDB:DatabaseName");

        _database = client.GetDatabase(dbName);        
    }    
}
