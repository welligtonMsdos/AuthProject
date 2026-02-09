using Auth10Api.Application.Common;
using Auth10Api.Application.Dtos;
using Auth10Api.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth10Api.Presentation.Controllers;


[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;
    private readonly ITokenService _tokenService;
   
    public UsersController(IUserService service, 
                           ITokenService tokenService)
    {
        _service = service;
        _tokenService = tokenService;       
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
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] UserCreateDto userCreateDto)
    {
        var newUser = await _service.CreateAsync(userCreateDto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = newUser._id.ToString() },
            Result<UserDto>.Ok(newUser, "User successfully created!")
        );
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _service.GetAllAsync();

        return Ok(Result<IEnumerable<UserDto>>.Ok(users));
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await _service.GetByIdAsync(id);

        if (user == null) return NotFound(Result<object>.Failure("User not found."));

        return Ok(Result<UserDto>.Ok(user));
    }

    [Authorize]
    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetByEmail(string email)
    {
        var user = await _service.GetByEmailAsync(email);

        if (user == null) return NotFound(Result<object>.Failure("User not found."));

        return Ok(Result<UserDto>.Ok(user));
    }    

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UserUpdateDto userUpdateDto)
    {
        var updateUser = await _service.UpdateAsync(userUpdateDto);

        if (updateUser == null)
            return NotFound(Result<object>.Failure("User not found for update."));

        return Ok(Result<UserDto>.Ok(updateUser, "User successfully updated!"));
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deletedUser = await _service.DeleteByIdAsync(id);

        if (!deletedUser)
            return NotFound(Result<object>.Failure("User not found for deletion."));

        return Ok(Result<bool>.Ok(true, "User removed successfully!"));
    }
}
