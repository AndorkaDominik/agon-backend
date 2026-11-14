using FluentValidation;
using Transfer.Dtos;

public class CoachingRequestValidator : AbstractValidator<CoachingRequestDto>
{
    public CoachingRequestValidator()
    {
        RuleFor(x => x.TrainerId)
            .GreaterThan(0).WithMessage("TrainerId must be greater than 0.");

        RuleFor(x => x.Message)
            .MaximumLength(500).WithMessage("Message cannot exceed 500 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Message));
    }
}