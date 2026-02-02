using Auth10Api.Application.Common;
using Auth10Api.Application.Dtos;
using Auth10Api.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Runtime.CompilerServices;

namespace Auth10Api.Presentation.Controllers;


[ApiController]
[Route("api/[controller]")]
public class UserController : Controller
{
    private readonly IUserService _service;
    private readonly ITokenService _tokenService;
    private readonly IRabbitMQService _rabbitMQService;
    private readonly IMongoClient _mongoClient;
    public UserController(IUserService service, 
                          ITokenService tokenService,
                          IRabbitMQService rabbitMQService,
                          IMongoClient mongoClient)
    {
        _service = service;
        _tokenService = tokenService;
        _rabbitMQService = rabbitMQService;
        _mongoClient = mongoClient;
    }

    [HttpPost("[Action]")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
    {
        var user = await _service.GetDataLoginAsync(userLoginDto);

        if (user == null) return Unauthorized();

        var token = _tokenService.GenerateToken(user);

        return Ok(Result<Task<string>>.Ok(token));
    }

    [Authorize]
    [HttpGet("[Action]")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _service.GetUsersAsync();

        return Ok(Result<IEnumerable<UserDto>>.Ok(users));
    }

    [Authorize]
    [HttpGet("[Action]/{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        var user = await _service.GetUserByIdAsync(id);

        if (user == null) return NotFound(Result<object>.Failure("User not found."));

        return Ok(Result<UserDto>.Ok(user));
    }

    [Authorize]
    [HttpGet("[Action]/{email}")]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        var user = await _service.GetUserByEmailAsync(email);

        if (user == null) return NotFound(Result<object>.Failure("User not found."));

        return Ok(Result<UserDto>.Ok(user));
    }

    [Authorize]
    [HttpPost("[Action]")]
    public async Task<IActionResult> AddUser([FromBody] UserCreateDto user)
    {
        using var session = await _mongoClient.StartSessionAsync();

        session.StartTransaction();

        try
        {
            var newUser = await _service.AddUserAsync(user, session);

            await _rabbitMQService.AddUserDtoAsync(newUser);

            await session.CommitTransactionAsync();

            return CreatedAtAction(
                nameof(GetUserById),
                new { id = newUser._id },
                Result<UserDto>.Ok(newUser, "User successfully created!")
        );
        }
        catch (Exception ex)
        {
            if (session.IsInTransaction)
                await session.AbortTransactionAsync();

            throw;
        }

        //var newUser = await _service.AddUserAsync(user);

        //var resultRabbitMQ = await _rabbitMQService.AddUserDtoAsync(newUser);

        //return CreatedAtAction(nameof(GetUserById), new { id = newUser._id }, Result<UserDto>.Ok(newUser, "User successfully created!"));
    }

    [Authorize]
    [HttpPut("[Action]")]
    public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDto user)
    {
        var updateUser = await _service.UpdateUserAsync(user);

        if (updateUser == null)
            return NotFound(Result<object>.Failure("User not found for update."));

        return Ok(Result<UserDto>.Ok(updateUser, "User successfully updated!"));
    }

    [Authorize]
    [HttpDelete("[Action]/{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var deletedUser = await _service.DeleteUserByIdAsync(id);

        if (!deletedUser)
            return NotFound(Result<object>.Failure("User not found for deletion."));

        return Ok(Result<bool>.Ok(true, "User removed successfully!"));
    }
}
