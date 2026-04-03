using Auth10Api.Application.Dtos;
using FluentValidation;

namespace Auth10Api.Application.Validators;

public class UserUpdateValidator : AbstractValidator<UserUpdateDto>
{
    public UserUpdateValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required!");
        RuleFor(x => x.Name).MinimumLength(3).WithMessage("Name must be at least 3 characters.");
        RuleFor(x => x.Name).MaximumLength(50).WithMessage("Name must be at most 50 characters.");

        RuleFor(x => x.Email).NotEmpty().WithMessage("Email required!");
        RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid email!");
        RuleFor(x => x.Email).MinimumLength(12).WithMessage("Email must be at least 12 characters");
        RuleFor(x => x.Email).MaximumLength(50).WithMessage("Email must be at most 50 characters");
    }
}
