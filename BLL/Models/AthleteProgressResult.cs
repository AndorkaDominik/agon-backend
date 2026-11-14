namespace BLL.Models;

public record AthleteProgressResult(
    int Level,
    SkillProgress Strength,
    SkillProgress Endurance,
    SkillProgress BodyControl
);
