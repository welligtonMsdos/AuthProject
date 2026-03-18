using Auth10Api.Application.Common;
using Auth10Api.Application.Dtos;
using Auth10Api.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth10Api.Presentation.Controllers;


[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;
   
    public UsersController(IUserService service)
    {
        _service = service;       
    } 
   
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] UserCreateDto userCreateDto)
    {
        var newUser = await _service.PostAsync(userCreateDto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = newUser._id.ToString() },
            Result<UserDto>.Ok(newUser, "User successfully created!")
        );
    }
        
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? email)
    {
        if (!string.IsNullOrWhiteSpace(email))
        {
            var user = await _service.GetByEmailAsync(email);

            if (user is null) return NotFound(Result<object>.Failure("User not found."));

            return Ok(Result<UserDto>.Ok(user));
        }

        var users = await _service.GetAsync();

        return Ok(Result<IEnumerable<UserDto>>.Ok(users));
    }
   
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await _service.GetByIdAsync(id);

        if (user is null) return NotFound(Result<object>.Failure("User not found."));

        return Ok(Result<UserDto>.Ok(user));
    }
   
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] UserUpdateDto userUpdateDto)
    {
        var updateUser = await _service.PutAsync(id, userUpdateDto);

        if (updateUser is null)
            return NotFound(Result<object>.Failure("User not found for update."));

        return Ok(Result<UserDto>.Ok(updateUser, "User successfully updated!"));
    }
   
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deletedUser = await _service.DeleteAsync(id);

        if (!deletedUser)
            return NotFound(Result<object>.Failure("User not found for deletion."));

        return Ok(Result<bool>.Ok(true, "User removed successfully!"));
    }
}
