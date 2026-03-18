namespace Auth10Api.Application.Dtos;

public record UserCreateDto(string Name,
                            string Email,
                            string Password);
