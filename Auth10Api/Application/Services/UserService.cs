using Auth10Api.Application.Dtos;
using Auth10Api.Application.Interfaces;
using Auth10Api.Domain.Entities;
using Auth10Api.Domain.Interfaces;
using AutoMapper;
using MongoDB.Driver;
using System.Text.Json;

namespace Auth10Api.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;    
    private readonly IMongoClient _mongoClient;

    public UserService(IUserRepository repository, 
                       IMapper mapper,
                       IMongoClient mongoClient)
    {
        _repository = repository;
        _mapper = mapper;       
        _mongoClient = mongoClient;
    }

    public async Task<UserDto> CreateAsync(UserCreateDto userCreateDto)
    {
        using var session = await _mongoClient.StartSessionAsync();

        session.StartTransaction();

        try
        {
            var user = _mapper.Map<User>(userCreateDto);

            user.Active = true;

            user.LastAccess = DateTime.Now;

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            var newUser = await _repository.CreateAsync(user, session);

            var outboxMsg = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = "UserCreated",
                Content = JsonSerializer.Serialize(newUser),
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddOutboxMessage(outboxMsg, session);
            
            await session.CommitTransactionAsync();

            return _mapper.Map<UserDto>(newUser);
        }
        catch (Exception)
        {
            await session.AbortTransactionAsync();
            throw;
        } 
    }

    public async Task<ICollection<UserDto>> GetAllAsync()
    {
        return _mapper.Map<ICollection<UserDto>>(await _repository.GetAllAsync());
    }

    public async Task<UserDto> GetByIdAsync(string id)
    {
        return _mapper.Map<UserDto>(await _repository.GetByIdAsync(id));
    }

    public async Task<UserDto> GetByEmailAsync(string email)
    {
        return _mapper.Map<UserDto>(await _repository.GetByEmailAsync(email));
    }

    public async Task<UserDataLoginDto> GetDataLoginAsync(UserLoginDto userLoginDto)
    {
        var user = await _repository.GetDataLoginAsync(userLoginDto);

        if (user == null) throw new Exception("User not found");

        return _mapper.Map<UserDataLoginDto>(user);
    }

    public async Task<UserDto> UpdateAsync(UserUpdateDto userUpdated)
    {
        var user = _mapper.Map<User>(userUpdated);

        return _mapper.Map<UserDto>(await _repository.UpdateAsync(user));
    }

    public async Task<bool> DeleteByIdAsync(string id)
    {
        var user = await _repository.GetByIdAsync(id);

        if (user == null) throw new Exception("User not found");

        return await _repository.DeleteByIdAsync(id);
    }

}
