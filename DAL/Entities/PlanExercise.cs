namespace DAL.Entities;

public class PlanExercise
{
    public int Id { get; set; }
    public int WorkoutPlanId { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public int OrderIndex { get; set; }

    public ExerciseType ExerciseType { get; set; }

    // strength
    public double? TargetWeight { get; set; } // in kilograms
    public int? TargetReps { get; set; } // bodyweight or strength

    // cardio
    public double? TargetDuration { get; set; } // in minutes
    public double? TargetDistance { get; set; } // in kilometers
}