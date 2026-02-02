using Auth10Api.Application.Dtos;
using Auth10Api.Domain.Entities;
using MongoDB.Driver;

namespace Auth10Api.Domain.Interfaces;

public interface IUserRepository: IRepository
{
    Task<User> AddUserAsync(User user, IClientSessionHandle session);
    Task<bool> DeleteUserByIdAsync(string id);
    Task<User> UpdateUserAsync(User userUpdated);
    Task<ICollection<User>> GetUsersAsync();
    Task<User> GetUserByIdAsync(string id);
    Task<User> GetUserByEmailAsync(string email);
    Task<User> GetDataLoginAsync(UserLoginDto userLoginDto);
}
