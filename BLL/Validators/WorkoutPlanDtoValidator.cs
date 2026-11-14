using FluentValidation;
using Transfer.Dtos;

namespace BLL.Validators;

public class WorkoutPlanDtoValidator : AbstractValidator<WorkoutPlanDto>
{
    public WorkoutPlanDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Plan name is required.")
            .MaximumLength(100).WithMessage("Plan name must not exceed 100 characters.");

        RuleFor(x => x.Exercises)
            .NotEmpty().WithMessage("At least one exercise is required.");

        RuleForEach(x => x.Exercises).ChildRules(exercise =>
        {
            exercise.RuleFor(e => e.ExerciseName)
                .NotEmpty().WithMessage("Exercise name is required.")
                .MaximumLength(50).WithMessage("Exercise name must not exceed 50 characters.");

            exercise.RuleFor(e => e.ExerciseType)
                .IsInEnum().WithMessage("Invalid exercise type.");

            // strength
            exercise.When(e => e.ExerciseType == ExerciseType.Strength, () =>
            {
                exercise.RuleFor(e => e.TargetWeight)
                    .NotNull().WithMessage("Weight is required for strength exercises.")
                    .GreaterThan(0).WithMessage("Weight must be greater than 0.");

                exercise.RuleFor(e => e.TargetReps)
                    .NotNull().WithMessage("Reps is required for strength exercises.")
                    .GreaterThan(0).WithMessage("Reps must be greater than 0.");
            });

            // cardio
            exercise.When(e => e.ExerciseType == ExerciseType.Cardio, () =>
            {
                exercise.RuleFor(e => e.TargetDuration)
                    .NotNull().WithMessage("Duration is required for cardio exercises.")
                    .GreaterThan(0).WithMessage("Duration must be greater than 0.");
                exercise.RuleFor(e => e.TargetDistance)
                    .NotNull().WithMessage("Distance is required for cardio exercises.")
                    .GreaterThan(0).WithMessage("Distance must be greater than 0.");
            });

            // bodyweight
            exercise.When(e => e.ExerciseType == ExerciseType.Bodyweight, () =>
            {
                exercise.RuleFor(e => e.TargetReps)
                    .NotNull().WithMessage("Reps is required for bodyweight exercises.")
                    .GreaterThan(0);
            });
        });
    }
}
