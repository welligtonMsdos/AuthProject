using Auth10Api.Application.Dtos;
using Auth10Api.Domain.Entities;
using Auth10Api.Domain.Interfaces;
using Auth10Api.Infrastruture.Data;
using MongoDB.Driver;

namespace Auth10Api.Infrastruture.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthContext _context;

    public UserRepository(AuthContext context)
    {
        _context = context;
    }

    public async Task<User> AddUserAsync(User user)
    {
        await _context.Users.InsertOneAsync(user);

        return user;
    }

    public async Task<bool> DeleteUserByIdAsync(string id)
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

    public async Task<User> GetUserByEmailAsync(string email)
    {
        var user = await _context.Users
            .Find(u => u.Email == email && u.Active)
            .FirstOrDefaultAsync();

        return user;
    }

    public async Task<User> GetUserByIdAsync(string id)
    {
        var user = await _context.Users
            .Find(u => u._id == id && u.Active)
            .FirstOrDefaultAsync();

        return user;
    }

    public async Task<ICollection<User>> GetUsersAsync()
    {
        var users = await _context.Users.Find(u => u.Active).ToListAsync();

        return users;
    }

    public async Task<User> UpdateUserAsync(User userUpdated)
    {
        await _context.Users.UpdateOneAsync(
            u => u._id == userUpdated._id,
            Builders<User>.Update
                .Set(u => u.Name, userUpdated.Name)
                .Set(u => u.Email, userUpdated.Email)
                .Set(u => u.LastAccess, DateTime.Now));

        var user = await _context.Users.Find(u => u._id == userUpdated._id).FirstOrDefaultAsync();

        return user;
    }
}
