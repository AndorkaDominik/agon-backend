using BLL.Constants;
using BLL.Models;
using BLL.Services;

namespace Tests;

public class GeneratedTestsWithSourceCode
{
    private readonly WorkoutXpCalculatorService _sut = new();

    [Theory]
    [InlineData(0)]
    public void CalculateWorkoutXp_ReturnsZero_ForEmptySession(int _)
    {
        var actual = _sut.CalculateWorkoutXp(TestHelpers.CreateSession());

        var expected = new XpResult
        {
            StrengthXp = 0,
            EnduranceXp = 0,
            BodyControlXp = 0
        };

        Assert.Equivalent(expected, actual);
    }

    [Theory]
    [InlineData(25d, 16, 20)]
    [InlineData(-5d, 10, 0)]
    [InlineData(2000d, 600, XpConstants.MaxExerciseXp)]
    public void CalculateWorkoutXp_CalculatesStrengthXp(double weight, int reps, int expectedStrengthXp)
    {
        var actual = _sut.CalculateWorkoutXp(TestHelpers.CreateSession(TestHelpers.Strength(weight, reps)));

        var expected = new XpResult
        {
            StrengthXp = expectedStrengthXp,
            EnduranceXp = 0,
            BodyControlXp = 0
        };

        Assert.Equivalent(expected, actual);
    }

    [Theory]
    [InlineData(30d, 5d, 44)]
    [InlineData(0d, 5d, 0)]
    [InlineData(100d, 200d, XpConstants.MaxExerciseXp)]
    public void CalculateWorkoutXp_CalculatesCardioXp(double durationMinutes, double distanceKm, int expectedEnduranceXp)
    {
        var actual = _sut.CalculateWorkoutXp(TestHelpers.CreateSession(TestHelpers.Cardio(durationMinutes, distanceKm)));

        var expected = new XpResult
        {
            StrengthXp = 0,
            EnduranceXp = expectedEnduranceXp,
            BodyControlXp = 0
        };

        Assert.Equivalent(expected, actual);
    }

    [Theory]
    [InlineData(10, 17)]
    [InlineData(0, 0)]
    [InlineData(10000, XpConstants.MaxExerciseXp)]
    public void CalculateWorkoutXp_CalculatesBodyweightXp(int reps, int expectedBodyControlXp)
    {
        var actual = _sut.CalculateWorkoutXp(TestHelpers.CreateSession(TestHelpers.Bodyweight(reps)));

        var expected = new XpResult
        {
            StrengthXp = 0,
            EnduranceXp = 0,
            BodyControlXp = expectedBodyControlXp
        };

        Assert.Equivalent(expected, actual);
    }

    [Theory]
    [InlineData(0)]
    public void CalculateWorkoutXp_AggregatesMixedWorkout(int _)
    {
        var actual = _sut.CalculateWorkoutXp(TestHelpers.CreateSession(
            TestHelpers.Strength(25d, 16),
            TestHelpers.Strength(36d, 25),
            TestHelpers.Cardio(30d, 5d),
            TestHelpers.Cardio(10d, 1d),
            TestHelpers.Bodyweight(10),
            TestHelpers.Bodyweight(1)));

        var expected = new XpResult
        {
            StrengthXp = 50,
            EnduranceXp = 51,
            BodyControlXp = 20
        };

        Assert.Equivalent(expected, actual);
    }
}
