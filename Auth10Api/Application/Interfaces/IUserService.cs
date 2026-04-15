using Auth10Api.Application.Dtos;

namespace Auth10Api.Application.Interfaces;

public interface IUserService
{
    Task<UserDto> PostAsync(UserCreateDto userCreateDto);
    Task<ICollection<UserDto>> GetAsync();
    Task<UserDto> GetByIdAsync(string id);
    Task<UserDto> GetByEmailAsync(string email);
    Task<UserDataLoginDto> GetDataLoginAsync(UserLoginDto userLoginDto);    
    Task<UserDto> PutAsync(string id, UserUpdateDto userUpdated);
    Task<bool> DeleteAsync(string id);
    Task<bool> ExistsByEmailAsync(string email);
}
