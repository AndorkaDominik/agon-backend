using BLL.Models;
using DAL.Entities;
using BLL.Constants;

namespace BLL.Services;

public class WorkoutXpCalculatorService
{

    public XpResult CalculateWorkoutXp(WorkoutSession workout)
    {
        double strengthXp = 0;
        double enduranceXp = 0;
        double bodyControlXp = 0;

        if(workout.Exercises == null || !workout.Exercises.Any())
        {
            return new XpResult
            {
                StrengthXp = 0,
                EnduranceXp = 0,
                BodyControlXp = 0
            };
        }

        foreach (var exercise in workout.Exercises)
        {
            switch (exercise.ExerciseType)
            {
                case ExerciseType.Strength:
                    strengthXp += CalculateStrengthXp(exercise);
                    break;

                case ExerciseType.Cardio:
                    enduranceXp += CalculateCardioXp(exercise);
                    break;

                case ExerciseType.Bodyweight:
                    bodyControlXp += CalculateBodyweightXp(exercise);
                    break;
            }
        }

        return new XpResult
        {
            StrengthXp = LimitWorkoutXp(strengthXp),
            EnduranceXp = LimitWorkoutXp(enduranceXp),
            BodyControlXp = LimitWorkoutXp(bodyControlXp)
        };
    }

    private int LimitWorkoutXp(double xp)
    {
        return (int)Math.Round(Math.Min(xp, XpConstants.MaxExerciseXp));
    }

    private double CalculateStrengthXp(SessionExercise exercise)
    {
        if (exercise.Weight <= 0 || exercise.Reps <= 0) return 0;

        double xp = Math.Sqrt(exercise.Weight.Value * exercise.Reps.Value);

        return Math.Min(xp, XpConstants.MaxExerciseXp);
    }

    private double CalculateBodyweightXp(SessionExercise exercise)
    {
        double reps = exercise.Reps ?? 0;

        if (reps <= 0) return 0;

        double xp = XpConstants.BodyweightBaseMultiplier * Math.Pow(reps, XpConstants.BodyweightExponent);

        return Math.Min(xp, XpConstants.MaxExerciseXp);
    }

    private double CalculateCardioXp(SessionExercise exercise)
    {
        double durationMinutes = exercise.Duration ?? 0;
        double distanceKm = exercise.Distance ?? 0;

        if (durationMinutes <= 0 || distanceKm <= 0) return 0;

        double hours = durationMinutes / 60.0;
        double speed = distanceKm / hours;

        double baseXp = distanceKm * XpConstants.CardioDistanceMultiplier;

        double intensityMultiplier = Math.Clamp(
            XpConstants.CardioBaseIntensity + speed * XpConstants.CardioSpeedIntensityFactor,
            XpConstants.CardioIntensityMin,
            XpConstants.CardioIntensityMax);

        double xp = baseXp * intensityMultiplier;

        return Math.Min(xp, XpConstants.MaxExerciseXp);
    }
}
