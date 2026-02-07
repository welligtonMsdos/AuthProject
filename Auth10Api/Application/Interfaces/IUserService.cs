using Auth10Api.Application.Dtos;

namespace Auth10Api.Application.Interfaces;

public interface IUserService
{
    Task<UserDto> CreateAsync(UserCreateDto userCreateDto);
    Task<ICollection<UserDto>> GetAllAsync();
    Task<UserDto> GetByIdAsync(string id);
    Task<UserDto> GetByEmailAsync(string email);
    Task<UserDataLoginDto> GetDataLoginAsync(UserLoginDto userLoginDto);    
    Task<UserDto> UpdateAsync(UserUpdateDto userUpdated);
    Task<bool> DeleteByIdAsync(string id);
}
