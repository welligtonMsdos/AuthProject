using Auth10Api.Application.Dtos;
using Auth10Api.Application.Interfaces;
using Auth10Api.Domain.Entities;
using Auth10Api.Domain.Interfaces;
using AutoMapper;
using MongoDB.Driver;

namespace Auth10Api.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository repository, 
                       IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<UserDto> AddUserAsync(UserCreateDto userCreateDto, IClientSessionHandle session)
    {
        var user = _mapper.Map<User>(userCreateDto);

        user.Active = true;

        user.LastAccess = DateTime.Now;

        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

        return _mapper.Map<UserDto>(await _repository.AddUserAsync(user, session));
    }

    public async Task<bool> DeleteUserByIdAsync(string id)
    {
        var user = await _repository.GetUserByIdAsync(id);

        if (user == null) throw new Exception("User not found");

        return await _repository.DeleteUserByIdAsync(id);
    }

    public async Task<UserDataLoginDto> GetDataLoginAsync(UserLoginDto userLoginDto)
    {
        var user = await _repository.GetDataLoginAsync(userLoginDto);

        if (user == null) throw new Exception("User not found");

        return _mapper.Map<UserDataLoginDto>(user);
    }

    public async Task<UserDto> GetUserByEmailAsync(string email)
    {
        return _mapper.Map<UserDto>(await _repository.GetUserByEmailAsync(email));
    }

    public async Task<UserDto> GetUserByIdAsync(string id)
    {
        return _mapper.Map<UserDto>(await _repository.GetUserByIdAsync(id));
    }

    public async Task<ICollection<UserDto>> GetUsersAsync()
    {
        return _mapper.Map<ICollection<UserDto>>(await _repository.GetUsersAsync());
    }

    public async Task<UserDto> UpdateUserAsync(UserUpdateDto userUpdated)
    {
        var user = _mapper.Map<User>(userUpdated);

        return _mapper.Map<UserDto>(await _repository.UpdateUserAsync(user));
    }
}
