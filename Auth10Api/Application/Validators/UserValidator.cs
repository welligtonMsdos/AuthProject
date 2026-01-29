using Auth10Api.Application.Dtos;
using FluentValidation;

namespace Auth10Api.Application.Validators;

public class UserValidator : AbstractValidator<UserDto>
{
    public UserValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email cannot be empty!");
        RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid email!");
        RuleFor(x => x.Email).MinimumLength(12).WithMessage("Email must be at least 12 characters");
        RuleFor(x => x.Email).MaximumLength(50).WithMessage("Email must be at most 50 characters");
    }
}
