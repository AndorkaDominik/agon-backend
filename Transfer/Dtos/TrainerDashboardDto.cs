using System;
using System.Collections.Generic;
using System.Text;

namespace Transfer.Dtos;

public class TrainerDashboardDto
{
    public TrainerDashboardInfoDto Info { get; set; }
    public List<CoachingRequestDto> Requests { get; set; }
    public List<AthleteDto> MyAthletes { get; set; }
}
