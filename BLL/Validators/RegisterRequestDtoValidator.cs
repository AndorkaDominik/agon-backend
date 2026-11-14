using FluentValidation;
using Transfer.Dtos;

namespace BLL.Validators;

public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

        RuleFor(x => x.PasswordConfirm)
            .Equal(x => x.Password).WithMessage("Passwords do not match.");

        RuleFor(x => x.UserType)
            .NotEmpty().WithMessage("User type is required.")
            .Must(x => x == "Athlete" || x == "Trainer")
            .WithMessage("Invalid user type.");

        RuleFor(x => x.Specialization)
            .NotEmpty().When(x => x.UserType == "Trainer")
            .WithMessage("Specialization is required for trainers.");
    }
}
