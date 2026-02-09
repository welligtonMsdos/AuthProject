namespace Auth10Api.Application.Dtos;

public sealed record UserDataLoginDto(string _id,
                                      string Name,
                                      string Email,
                                      string Token)
{
    public UserDataLoginDto() : this(string.Empty, 
                                     string.Empty, 
                                     string.Empty, 
                                     string.Empty) { }
}
