namespace Auth10Api.Application.Dtos;

public sealed record UserUpdateDto(string _id, 
                                   string Name,
                                   string Email);
