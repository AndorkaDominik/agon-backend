namespace Transfer.Dtos;

public class ActivityDayDto
{
    public DateTime Date { get; set; }
    public string WorkoutName { get; set; }
    public int ExerciseCount { get; set; }
    public int? TotalReps { get; set; } // only for bodyweight exercises
    public double? TotalWeight { get; set; } // only for strength exercises
    public double? TotalDistance { get; set; } // only for cardio exercises
}