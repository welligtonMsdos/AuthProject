using Auth10Api.Application.Dtos;

namespace Auth10Api.Application.Interfaces;

public interface IRabbitMQService: IService
{
    Task<bool> AddUserDtoAsync(UserDto userDto);
}
