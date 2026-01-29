using Auth10Api.Application.Dtos;
using FluentValidation;

namespace Auth10Api.Application.Validators;

public class UserCreateValidator : AbstractValidator<UserCreateDto>
{
    public UserCreateValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required!");
        RuleFor(x => x.Name).MinimumLength(3).WithMessage("Name must be at least 3 characters.");
        RuleFor(x => x.Name).MaximumLength(50).WithMessage("Name must be at most 50 characters.");

        RuleFor(x => x.Email).NotEmpty().WithMessage("Email s required!");
        RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid email!");
        RuleFor(x => x.Email).MinimumLength(12).WithMessage("Email must be at least 12 characters");
        RuleFor(x => x.Email).MaximumLength(50).WithMessage("Email must be at most 50 characters");

        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required!");
        RuleFor(x => x.Password).MinimumLength(6).WithMessage("Password must be at least 6 characters.");
    }
}
