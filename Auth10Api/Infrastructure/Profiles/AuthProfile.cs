using Auth10Api.Application.Dtos;
using Auth10Api.Domain.Entities;
using AutoMapper;

namespace Auth10Api.Infrastructure.Profiles;

public class AuthProfile: Profile
{
    public AuthProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<User, UserCreateDto>().ReverseMap();
        CreateMap<User, UserLoginDto>().ReverseMap();
        CreateMap<User, UserDataLoginDto>().ReverseMap();
        CreateMap<User, UserUpdateDto>().ReverseMap();
    }
}
