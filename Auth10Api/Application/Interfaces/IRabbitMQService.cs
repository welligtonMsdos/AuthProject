using Auth10Api.Domain.Entities;

namespace Auth10Api.Application.Interfaces;

public interface IRabbitMQService
{
    Task<bool> AddUserDtoAsync(User user);
    //Task<bool> PublishMessageAsync(string message);
}
