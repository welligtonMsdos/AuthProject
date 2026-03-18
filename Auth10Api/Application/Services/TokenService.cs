using Auth10Api.Application.Dtos;
using Auth10Api.Application.Interfaces;
using Auth10Api.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth10Api.Application.Services;

public class TokenService : ITokenService
{
    public async Task<string> GenerateToken(UserDataLoginDto userDataLoginDto)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var keyVault = Environment.GetEnvironmentVariable("JwtSettings__Key");

        ArgumentNullException.ThrowIfNull(keyVault);

        var key = Encoding.ASCII.GetBytes(keyVault);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
           {
                    new Claim("email", userDataLoginDto.Email),
                    new Claim("id", userDataLoginDto._id),
                    new Claim("name", userDataLoginDto.Name),
           }),
            Expires = DateTime.UtcNow.AddHours(2),
            //Issuer = "http://13.59.37.186:5011",
            Issuer = "http://localhost:5011",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
