namespace BLL.Constants;

public static class XpConstants
{
    public const int MaxExerciseXp = 1000;

    public const int BaseXp = 100;
    public const double Growth = 15;
    public const double LevelExponent = 1.5;
    public const double OverallAverageWeight = 0.7;
    public const double OverallLowestWeight = 0.3;

    public const double BodyweightBaseMultiplier = 3.0;
    public const double BodyweightExponent = 0.75;
    
    public const double CardioDistanceMultiplier = 8.0;
    public const double CardioBaseIntensity = 0.6;
    public const double CardioSpeedIntensityFactor = 0.05;
    public const double CardioIntensityMin = 0.7;
    public const double CardioIntensityMax = 1.5;
}