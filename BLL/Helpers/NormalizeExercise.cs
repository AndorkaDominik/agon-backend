using DAL.Entities;

namespace BLL.Helpers;

public static class NormalizeExercisesHelper
{
    public static void NormalizeSessionExercises(IEnumerable<SessionExercise> exercises)
    {
        foreach (var exercise in exercises)
        {
            switch (exercise.ExerciseType)
            {
                case ExerciseType.Strength:
                    exercise.Duration = null;
                    exercise.Distance = null;
                    break;

                case ExerciseType.Cardio:
                    exercise.Weight = null;
                    exercise.Reps = null;
                    break;

                case ExerciseType.Bodyweight:
                    exercise.Weight = null;
                    exercise.Duration = null;
                    exercise.Distance = null;
                    break;
            }
        }
    }

    public static void NormalizePlanExercises(IEnumerable<PlanExercise> exercises)
    {
        foreach (var exercise in exercises)
        {
            switch (exercise.ExerciseType)
            {
                case ExerciseType.Strength:
                    exercise.TargetDuration = null;
                    exercise.TargetDistance = null;
                    break;

                case ExerciseType.Cardio:
                    exercise.TargetWeight = null;
                    exercise.TargetReps = null;
                    break;

                case ExerciseType.Bodyweight:
                    exercise.TargetWeight = null;
                    exercise.TargetDuration = null;
                    exercise.TargetDistance = null;
                    break;
            }
        }
    }
}