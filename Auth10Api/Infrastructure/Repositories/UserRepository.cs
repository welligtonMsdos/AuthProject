using Auth10Api.Application.Dtos;
using Auth10Api.Domain.Entities;
using Auth10Api.Domain.Interfaces;
using Auth10Api.Infrastructure.Data;
using MongoDB.Driver;

namespace Auth10Api.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthContext _context;

    public UserRepository(AuthContext context)
    {
        _context = context;
    }

    public async Task<User> CreateAsync(User obj, 
                                        IClientSessionHandle session)
    {
        await _context.Users.InsertOneAsync(session, obj);

        return obj;
    }

    public async Task<ICollection<User>> GetAllAsync()
    {
        var users = await _context.Users.Find(u => u.Active).ToListAsync();

        return users;
    }

    public async Task<User> GetByIdAsync(string id)
    {
        var user = await _context.Users
            .Find(u => u._id == id && u.Active)
            .FirstOrDefaultAsync();

        return user;
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        var user = await _context.Users
            .Find(u => u.Email == email && u.Active)
            .FirstOrDefaultAsync();

        return user;
    }

    public async Task<User> GetDataLoginAsync(UserLoginDto userLoginDto)
    {
        var user = await _context.Users
            .Find(u => u.Email == userLoginDto.Email && u.Active)
            .FirstOrDefaultAsync();

        if (user == null) throw new Exception("User not found.");

        bool verified = BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.Password);

        if (!verified) throw new Exception("Invalid password.");

        return user;
    }

    public async Task<User> UpdateAsync(User obj)
    {
        await _context.Users.UpdateOneAsync(
            u => u._id == obj._id,
            Builders<User>.Update
                .Set(u => u.Name, obj.Name)
                .Set(u => u.Email, obj.Email)
                .Set(u => u.LastAccess, DateTime.Now));

        var user = await _context.Users.Find(u => u._id == obj._id).FirstOrDefaultAsync();

        return user;
    }

    public async Task<bool> DeleteByIdAsync(string id)
    {
        try
        {
            await _context.Users.UpdateOneAsync(
            u => u._id == id,
            Builders<User>.Update
                .Set(u => u.Active, false));

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> AddOutboxMessage(OutboxMessage outboxMessage, IClientSessionHandle session)
    {
        try
        {
            await _context.OutboxMessages.InsertOneAsync(session, outboxMessage);            

            return true;
        }
        catch (Exception ex)
        {
            var msg = ex.Message;

            return false;
        }
    }
}
