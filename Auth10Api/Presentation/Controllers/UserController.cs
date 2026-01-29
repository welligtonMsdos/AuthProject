using Auth10Api.Application.Common;
using Auth10Api.Application.Dtos;
using Auth10Api.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Auth10Api.Presentation.Controllers;


[ApiController]
[Route("api/[controller]")]
public class UserController : Controller
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }

    [HttpGet("[Action]")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _service.GetUsersAsync();

        return Ok(Result<IEnumerable<UserDto>>.Ok(users));
    }

    [HttpGet("[Action]/{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        var user = await _service.GetUserByIdAsync(id);

        if (user == null) return NotFound(Result<object>.Failure("User not found."));

        return Ok(Result<UserDto>.Ok(user));
    }

    [HttpGet("[Action]/{email}")]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        var user = await _service.GetUserByEmailAsync(email);

        if (user == null) return NotFound(Result<object>.Failure("User not found."));

        return Ok(Result<UserDto>.Ok(user));
    }

    [HttpPost("[Action]")]
    public async Task<IActionResult> AddUser([FromBody] UserCreateDto user)
    {
        var newUser = await _service.AddUserAsync(user);

        return CreatedAtAction(nameof(GetUserById), new { id = newUser._id }, Result<UserDto>.Ok(newUser, "User successfully created!"));
    }

    [HttpPut("[Action]")]
    public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDto user)
    {
        var updateUser = await _service.UpdateUserAsync(user);

        if (updateUser == null)
            return NotFound(Result<object>.Failure("User not found for update."));

        return Ok(Result<UserDto>.Ok(updateUser, "User successfully updated!"));
    }

    [HttpDelete("[Action]/{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var deletedUser = await _service.DeleteUserByIdAsync(id);

        if (!deletedUser)
            return NotFound(Result<object>.Failure("User not found for deletion."));

        return Ok(Result<bool>.Ok(true, "User removed successfully!"));
    }
}
