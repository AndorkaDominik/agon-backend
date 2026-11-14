using BLL.Constants;
using BLL.Services;

namespace Tests;

public class GeneratedTestsWithOutSourceCode
{
    private readonly WorkoutXpCalculatorService _sut = new();

    [Theory]
    [InlineData(0)]
    public void CalculateWorkoutXp_ReturnsZero_ForEmptyExercises(int _)
    {
        var result = _sut.CalculateWorkoutXp(TestHelpers.CreateSession());

        AssertAllZero(result);
    }

    [Theory]
    [InlineData(0)]
    public void CalculateWorkoutXp_ReturnsZero_WhenExercisesListIsNull(int _)
    {
        var session = TestHelpers.CreateSession();
        session.Exercises = null!;

        var result = _sut.CalculateWorkoutXp(session);

        AssertAllZero(result);
    }

    [Theory]
    [InlineData(0)]
    public void CalculateWorkoutXp_Throws_ForNullWorkoutSession(int _)
    {
        Assert.ThrowsAny<Exception>(() => _sut.CalculateWorkoutXp(null!));
    }

    [Theory]
    [InlineData(20d, 10)]
    [InlineData(60d, 15)]
    public void CalculateWorkoutXp_OnlyStrengthCategoryChanges_ForStrengthOnlySession(double weight, int reps)
    {
        var result = _sut.CalculateWorkoutXp(TestHelpers.CreateSession(
            TestHelpers.Strength(weight, reps)));

        Assert.True(result.StrengthXp > 0);
        Assert.Equal(0, result.EnduranceXp);
        Assert.Equal(0, result.BodyControlXp);
    }

    [Theory]
    [InlineData(30d, 5d)]
    [InlineData(45d, 8d)]
    public void CalculateWorkoutXp_OnlyEnduranceCategoryChanges_ForCardioOnlySession(double durationMinutes, double distanceKm)
    {
        var result = _sut.CalculateWorkoutXp(TestHelpers.CreateSession(
            TestHelpers.Cardio(durationMinutes, distanceKm)));

        Assert.Equal(0, result.StrengthXp);
        Assert.True(result.EnduranceXp > 0);
        Assert.Equal(0, result.BodyControlXp);
    }

    [Theory]
    [InlineData(12)]
    [InlineData(30)]
    public void CalculateWorkoutXp_OnlyBodyControlCategoryChanges_ForBodyweightOnlySession(int reps)
    {
        var result = _sut.CalculateWorkoutXp(TestHelpers.CreateSession(
            TestHelpers.Bodyweight(reps)));

        Assert.Equal(0, result.StrengthXp);
        Assert.Equal(0, result.EnduranceXp);
        Assert.True(result.BodyControlXp > 0);
    }

    [Theory]
    [InlineData(0d, 10)]
    [InlineData(10d, 0)]
    [InlineData(-5d, 10)]
    [InlineData(10d, -1)]
    public void CalculateWorkoutXp_InvalidStrengthInput_GivesZeroContribution(double weight, int reps)
    {
        var result = _sut.CalculateWorkoutXp(TestHelpers.CreateSession(
            TestHelpers.Strength(weight, reps)));

        AssertAllZero(result);
    }

    [Theory]
    [InlineData(0d, 5d)]
    [InlineData(30d, 0d)]
    [InlineData(-1d, 5d)]
    [InlineData(30d, -1d)]
    public void CalculateWorkoutXp_InvalidCardioInput_GivesZeroContribution(double durationMinutes, double distanceKm)
    {
        var result = _sut.CalculateWorkoutXp(TestHelpers.CreateSession(
            TestHelpers.Cardio(durationMinutes, distanceKm)));

        AssertAllZero(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CalculateWorkoutXp_InvalidBodyweightInput_GivesZeroContribution(int reps)
    {
        var result = _sut.CalculateWorkoutXp(TestHelpers.CreateSession(
            TestHelpers.Bodyweight(reps)));

        AssertAllZero(result);
    }

    [Theory]
    [InlineData(0)] // Strength
    [InlineData(1)] // Cardio
    [InlineData(2)] // Bodyweight
    public void CalculateWorkoutXp_AggregatesAndLimitsPerCategory_ToMaxExerciseXp(int type)
    {
        var exercises = type switch
        {
            0 => new[]
            {
                TestHelpers.Strength(1_000_000d, 1_000_000),
                TestHelpers.Strength(1_000_000d, 1_000_000),
                TestHelpers.Strength(1_000_000d, 1_000_000)
            },
            1 => new[]
            {
                TestHelpers.Cardio(1_000_000d, 1_000_000d),
                TestHelpers.Cardio(1_000_000d, 1_000_000d),
                TestHelpers.Cardio(1_000_000d, 1_000_000d)
            },
            _ => new[]
            {
                TestHelpers.Bodyweight(1_000_000),
                TestHelpers.Bodyweight(1_000_000),
                TestHelpers.Bodyweight(1_000_000)
            }
        };

        var result = _sut.CalculateWorkoutXp(TestHelpers.CreateSession(exercises));

        switch (type)
        {
            case 0:
                Assert.Equal(XpConstants.MaxExerciseXp, result.StrengthXp);
                Assert.Equal(0, result.EnduranceXp);
                Assert.Equal(0, result.BodyControlXp);
                break;
            case 1:
                Assert.Equal(0, result.StrengthXp);
                Assert.Equal(XpConstants.MaxExerciseXp, result.EnduranceXp);
                Assert.Equal(0, result.BodyControlXp);
                break;
            default:
                Assert.Equal(0, result.StrengthXp);
                Assert.Equal(0, result.EnduranceXp);
                Assert.Equal(XpConstants.MaxExerciseXp, result.BodyControlXp);
                break;
        }
    }

    [Theory]
    [InlineData(100d, 20, 30d, 5d, 20)]
    [InlineData(60d, 15, 45d, 8d, 30)]
    public void CalculateWorkoutXp_MixedWorkout_ProcessesCategoriesIndependently(
        double strengthWeight, int strengthReps, double cardioDuration, double cardioDistance, int bodyweightReps)
    {
        var result = _sut.CalculateWorkoutXp(TestHelpers.CreateSession(
            TestHelpers.Strength(strengthWeight, strengthReps),
            TestHelpers.Cardio(cardioDuration, cardioDistance),
            TestHelpers.Bodyweight(bodyweightReps)));

        Assert.True(result.StrengthXp > 0);
        Assert.True(result.EnduranceXp > 0);
        Assert.True(result.BodyControlXp > 0);
    }

    private static void AssertAllZero(dynamic result)
    {
        Assert.Equal(0, (int)result.StrengthXp);
        Assert.Equal(0, (int)result.EnduranceXp);
        Assert.Equal(0, (int)result.BodyControlXp);
    }
}