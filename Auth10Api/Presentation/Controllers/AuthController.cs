using Auth10Api.Application.Common;
using Auth10Api.Application.Dtos;
using Auth10Api.Application.Interfaces;
using Auth10Api.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Auth10Api.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _service;
    private readonly ITokenService _tokenService;

    public AuthController(IUserService service, 
                          ITokenService tokenService)
    {
        _service = service;
        _tokenService = tokenService;
    }

    [HttpPost("[Action]")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
    {
        var user = await _service.GetDataLoginAsync(userLoginDto);

        if (user is null) return Unauthorized();

        var token = _tokenService.GenerateToken(user);

        return Ok(Result<Task<string>>.Ok(token));
    }    
}
