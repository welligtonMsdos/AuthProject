using Auth10Api.Application.Dtos;

namespace Auth10Api.Application.Interfaces;

public interface IUserService: IService
{
    Task<ICollection<UserDto>> GetUsersAsync();
    Task<UserDto> GetUserByIdAsync(string id);
    Task<UserDto> GetUserByEmailAsync(string email);
    Task<UserDataLoginDto> GetDataLoginAsync(UserLoginDto userLoginDto);
    Task<UserDto> AddUserAsync(UserCreateDto userCreateDto);
    Task<UserDto> UpdateUserAsync(UserUpdateDto userUpdated);
    Task<bool> DeleteUserByIdAsync(string id);
}
