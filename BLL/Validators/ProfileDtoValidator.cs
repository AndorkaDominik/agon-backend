using FluentValidation;
using Transfer.Dtos;

namespace BLL.Validators;

public class ProfileDtoValidator : AbstractValidator<ProfileDto>
{
    public ProfileDtoValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("User name is required.")
            .MinimumLength(3).WithMessage("User name must be at least 3 characters long.")
            .MaximumLength(50).WithMessage("User name must not exceed 50 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Specialization)
            .MaximumLength(100).WithMessage("Specialization must not exceed 100 characters.")
            .When(x => x.Specialization != null);

        RuleFor(x => x.Introduction)
            .MaximumLength(500).WithMessage("Introduction must not exceed 500 characters.")
            .When(x => x.Introduction != null);
    }
}