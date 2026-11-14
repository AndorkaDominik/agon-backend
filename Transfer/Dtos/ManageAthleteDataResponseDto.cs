using System;
using System.Collections.Generic;
using System.Text;

namespace Transfer.Dtos;
public class ManageAthleteDataResponseDto
{
    public List<WorkoutPlanDto> AssignedPlans { get; set; } = new();
    public ActivityWeekResponseDto ActivityWeek { get; set; } = new();
}
