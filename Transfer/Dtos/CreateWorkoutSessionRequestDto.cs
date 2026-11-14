using System;
using System.Collections.Generic;
using System.Text;

namespace Transfer.Dtos;

public class CreateWorkoutSessionRequestDto
{
    public int? PlanId { get; set; }
    public string WorkoutName { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public List<SessionExerciseDto> Exercises { get; set; } = new();

}
