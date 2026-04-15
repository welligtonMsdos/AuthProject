using Auth10Api.Application.Dtos;
using Auth10Api.Domain.Entities;
using MongoDB.Driver;

namespace Auth10Api.Domain.Interfaces;

public interface IUserRepository: IRepository<User>
{   
    Task<User> GetByEmailAsync(string email);
    Task<User> GetDataLoginAsync(UserLoginDto userLoginDto);
    Task<bool> AddOutboxMessage(OutboxMessage outboxMessage, IClientSessionHandle session);
    Task<bool> ExistsByEmailAsync(string email);
}
