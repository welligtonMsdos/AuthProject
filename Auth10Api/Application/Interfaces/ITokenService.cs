using Auth10Api.Application.Dtos;

namespace Auth10Api.Application.Interfaces;

public interface ITokenService
{
    Task<string> GenerateToken(UserDataLoginDto userDataLoginDto);
}
