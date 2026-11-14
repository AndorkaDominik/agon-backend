namespace DAL.Entities;

public class SessionExercise
{
    public int Id { get; set; }
    public int WorkoutSessionId { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public int OrderIndex { get; set; } = 0;

    public ExerciseType ExerciseType { get; set; }

    // strength
    public double? Weight { get; set; } // in kilograms
    public int? Reps { get; set; } // bodyweight or strength

    // cardio
    public double? Duration { get; set; } // in minutes
    public double? Distance { get; set; } // in kilometers
}