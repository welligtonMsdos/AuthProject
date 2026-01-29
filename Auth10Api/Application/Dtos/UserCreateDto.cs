namespace Auth10Api.Application.Dtos;

public sealed record UserCreateDto(string Name,
                                   string Email,
                                   string Password);
