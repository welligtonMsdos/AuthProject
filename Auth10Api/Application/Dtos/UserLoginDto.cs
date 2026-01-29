namespace Auth10Api.Application.Dtos;

public sealed record UserLoginDto(string Email,
                                  string Password)
{
    public UserLoginDto() : this(string.Empty, string.Empty) { }
}
