using BLL.Models;
using BLL.Constants;

namespace BLL.Services;

public class AthleteProgressCalculatorService
{
    public AthleteProgressResult GetAthleteProgress(
        int strengthXp,
        int enduranceXp,
        int bodyControlXp)
    {
        var strength = GetProgress(strengthXp);
        var endurance = GetProgress(enduranceXp);
        var bodyControl = GetProgress(bodyControlXp);

        int overallLevel = CalculateOverallLevel(
            strength.Level,
            endurance.Level,
            bodyControl.Level
        );

        return new AthleteProgressResult(
            overallLevel,
            strength,
            endurance,
            bodyControl
        );
    }

    public SkillProgress GetProgress(int totalXp)
    {
        int level = 1;
        int remainingXp = totalXp;

        while (true)
        {
            int requiredXp = GetRequiredXpForNextLevel(level);

            if (remainingXp < requiredXp)
            {
                return new SkillProgress
                {
                    Level = level,
                    CurrentXp = remainingXp,
                    RequiredXp = requiredXp,
                    TotalXp = totalXp,
                    ProgressPercentage = (double)remainingXp / requiredXp * 100
                };
            }

            remainingXp -= requiredXp;
            level++;
        }
    }

    private int GetRequiredXpForNextLevel(int level)
    {
        return (int)Math.Round(XpConstants.BaseXp + XpConstants.Growth * Math.Pow(level - 1, XpConstants.LevelExponent));
    }

    public int CalculateOverallLevel(
        int strengthLevel,
        int enduranceLevel,
        int bodyControlLevel)
    {
        double average = (strengthLevel + enduranceLevel + bodyControlLevel) / 3.0;

        int lowestLevel = Math.Min(strengthLevel, Math.Min(enduranceLevel, bodyControlLevel));

        double overall = average * XpConstants.OverallAverageWeight + lowestLevel * XpConstants.OverallLowestWeight;

        return (int)Math.Floor(overall);
    }
}
