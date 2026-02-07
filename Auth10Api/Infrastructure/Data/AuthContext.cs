using Auth10Api.Domain.Entities;
using MongoDB.Driver;

namespace Auth10Api.Infrastructure.Data;

public class AuthContext
{
    private readonly IMongoDatabase _database;

    public IMongoCollection<User> Users => _database.GetCollection<User>("User");

    public IMongoCollection<OutboxMessage> OutboxMessages => _database.GetCollection<OutboxMessage>("OutboxMessage");

    public AuthContext(IMongoClient client, IConfiguration config)
    {   
        var dbName = config.GetValue<string>("MongoDB:DatabaseName");

        _database = client.GetDatabase(dbName);        
    }    
}
