using Auth10Api.Application.Dtos;
using Auth10Api.Application.Extensions;
using Auth10Api.Application.Interfaces;
using Auth10Api.Application.Validators;
using Auth10Api.Domain.Entities;
using Auth10Api.Domain.Interfaces;
using FluentValidation;
using MongoDB.Driver;
using System.Text.Json;

namespace Auth10Api.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;    
    private readonly IMongoClient _mongoClient;
    private readonly UserCreateValidator _createValidator;
    private readonly UserUpdateValidator _updateValidator;

    public UserService(IUserRepository repository,                       
                       IMongoClient mongoClient,
                       UserCreateValidator createValidator,
                       UserUpdateValidator updateValidator)
    {
        _repository = repository;              
        _mongoClient = mongoClient;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<UserDto> PostAsync(UserCreateDto userCreateDto)
    {
        using var session = await _mongoClient.StartSessionAsync();

        session.StartTransaction();

        try
        {
            await _createValidator.ValidateAndThrowAsync(userCreateDto);

            var userCreated = userCreateDto.ToEntity();

            userCreated.Active = true;

            userCreated.LastAccess = DateTime.Now;

            userCreated.Password = BCrypt.Net.BCrypt.HashPassword(userCreated.Password);

            var newUser = await _repository.PostAsync(userCreated, session);

            var outboxMsg = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = "UserCreated",
                Content = JsonSerializer.Serialize(newUser),
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddOutboxMessage(outboxMsg, session);
            
            await session.CommitTransactionAsync();

            return newUser.ToUserDto();
        }
        catch (Exception)
        {
            await session.AbortTransactionAsync();

            throw;
        } 
    }

    public async Task<ICollection<UserDto>> GetAsync()
    {
        var user = await _repository.GetAsync();

        ArgumentNullException.ThrowIfNull(user);

        return user
            .Select(e => e.ToUserDto())
            .ToList();
    }

    public async Task<UserDto> GetByIdAsync(string id)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);

        var user = await _repository.GetByIdAsync(id);

        ArgumentNullException.ThrowIfNull(user);

        return user.ToUserDto();
    }

    public async Task<UserDto> GetByEmailAsync(string email)
    {
        ArgumentException.ThrowIfNullOrEmpty(email);

        var user = await _repository.GetByEmailAsync(email);        

        return user.ToUserDto();
    }

    public async Task<UserDataLoginDto> GetDataLoginAsync(UserLoginDto userLoginDto)
    {
        ArgumentNullException.ThrowIfNull(userLoginDto);

        var user = await _repository.GetDataLoginAsync(userLoginDto);

        ArgumentNullException.ThrowIfNull(user);

        return user.ToDataLoginDto();
    }

    public async Task<UserDto> PutAsync(string id, UserUpdateDto userUpdated)
    {
        await _updateValidator.ValidateAndThrowAsync(userUpdated);

        var entity = await _repository.GetByIdAsync(id);        

        entity.UpdateEntity(userUpdated);

        await _repository.PutAsync(entity);

        return entity.ToUserDto();
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var user = await _repository.GetByIdAsync(id);

        ArgumentNullException.ThrowIfNull(user);

        return await _repository.DeleteAsync(id);
    }
}
