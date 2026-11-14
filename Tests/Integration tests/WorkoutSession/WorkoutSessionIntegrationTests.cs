using System.Net;
using Transfer.Dtos;
using Tests.IntegrationTests.Infrastructure;
using Xunit.Abstractions;

namespace Tests.IntegrationTests.WorkoutSession;

public sealed class WorkoutSessionIntegrationTests(TestFixture fixture, ITestOutputHelper output) : IClassFixture<TestFixture>, IDisposable
{
    private readonly TestFixture _fixture = fixture;
    private readonly ITestOutputHelper _output = output;

    public void Dispose()
    {
        _fixture.CleanupAsync().GetAwaiter().GetResult();
    }

    [Fact]
    public async Task CreateWorkout_ShouldIncreaseXp()
    {
        var client = await _fixture.CreateAuthenticatedAthleteClientAsync();

        var progressBeforeResponse = await client.GetAsync("/api/WorkoutSession/dashboard-progress");
        Assert.Equal(HttpStatusCode.OK, progressBeforeResponse.StatusCode);
        var progressBefore = await progressBeforeResponse.Content.ReadFromJsonAsync<AthleteDashboardProgressDto>();
        Assert.NotNull(progressBefore);
        TestHelpers.LogStatus(_output, "dashboard-progress before workout", progressBeforeResponse.StatusCode);
        TestHelpers.LogProgress(_output, "progress before", progressBefore);

        var workoutDto = TestHelpers.CreateStrengthWorkoutSession("integration-user");
        _output.WriteLine($"new-workout request: Name='{workoutDto.WorkoutName}', ExerciseCount={workoutDto.Exercises.Count}");

        var createWorkoutResponse = await client.PostAsJsonAsync("/api/WorkoutSession/new-workout", workoutDto);
        Assert.Equal(HttpStatusCode.Created, createWorkoutResponse.StatusCode);
        TestHelpers.LogStatus(_output, "new-workout create", createWorkoutResponse.StatusCode);

        var createWorkoutResult = await createWorkoutResponse.Content.ReadFromJsonAsync<CreateWorkoutResponseDto>();
        Assert.NotNull(createWorkoutResult);
        Assert.Equal(workoutDto.WorkoutName, createWorkoutResult.Workout.WorkoutName);
        Assert.Equal(22, createWorkoutResult.XpGained.StrengthXp);
        Assert.Equal(0, createWorkoutResult.XpGained.EnduranceXp);
        Assert.Equal(0, createWorkoutResult.XpGained.BodyControlXp);

        var progressAfterResponse = await client.GetAsync("/api/WorkoutSession/dashboard-progress");
        Assert.Equal(HttpStatusCode.OK, progressAfterResponse.StatusCode);
        var progressAfter = await progressAfterResponse.Content.ReadFromJsonAsync<AthleteDashboardProgressDto>();
        Assert.NotNull(progressAfter);
        TestHelpers.LogStatus(_output, "dashboard-progress after workout", progressAfterResponse.StatusCode);
        TestHelpers.LogProgress(_output, "progress after", progressAfter);

        Assert.Equal(progressBefore.Strength.TotalXp + 22, progressAfter.Strength.TotalXp);
        Assert.Equal(progressBefore.Endurance.TotalXp, progressAfter.Endurance.TotalXp);
        Assert.Equal(progressBefore.BodyControl.TotalXp, progressAfter.BodyControl.TotalXp);
    }

    [Fact]
    public async Task CreateWorkout_WithHugeStrengthValues_ShouldCapStrengthXpAt1000()
    {
        var client = await _fixture.CreateAuthenticatedAthleteClientAsync();

        var progressBeforeResponse = await client.GetAsync("/api/WorkoutSession/dashboard-progress");
        Assert.Equal(HttpStatusCode.OK, progressBeforeResponse.StatusCode);
        var progressBefore = await progressBeforeResponse.Content.ReadFromJsonAsync<AthleteDashboardProgressDto>();
        Assert.NotNull(progressBefore);

        var workoutDto = TestHelpers.CreateStrengthWorkoutSession(
            userId: "integration-user",
            weight: 1_000_000,
            reps: 1_000_000);

        _output.WriteLine($"new-workout cap test request: Weight={workoutDto.Exercises[0].Weight}, Reps={workoutDto.Exercises[0].Reps}");

        var createWorkoutResponse = await client.PostAsJsonAsync("/api/WorkoutSession/new-workout", workoutDto);
        Assert.Equal(HttpStatusCode.Created, createWorkoutResponse.StatusCode);
        TestHelpers.LogStatus(_output, "new-workout cap test create", createWorkoutResponse.StatusCode);

        var progressAfterResponse = await client.GetAsync("/api/WorkoutSession/dashboard-progress");
        Assert.Equal(HttpStatusCode.OK, progressAfterResponse.StatusCode);
        var progressAfter = await progressAfterResponse.Content.ReadFromJsonAsync<AthleteDashboardProgressDto>();
        Assert.NotNull(progressAfter);
        TestHelpers.LogStatus(_output, "dashboard-progress cap test", progressAfterResponse.StatusCode);
        TestHelpers.LogProgress(_output, "progress after cap test", progressAfter);

        Assert.Equal(progressBefore.Strength.TotalXp + 1000, progressAfter.Strength.TotalXp);
        Assert.Equal(progressBefore.Endurance.TotalXp, progressAfter.Endurance.TotalXp);
        Assert.Equal(progressBefore.BodyControl.TotalXp, progressAfter.BodyControl.TotalXp);
    }

    [Fact]
    public async Task CreateWorkout_WithInvalidBodyweightReps_ShouldReturnBadRequest_AndKeepXpZero()
    {
        var client = await _fixture.CreateAuthenticatedAthleteClientAsync();

        var progressBeforeResponse = await client.GetAsync("/api/WorkoutSession/dashboard-progress");
        Assert.Equal(HttpStatusCode.OK, progressBeforeResponse.StatusCode);
        var progressBefore = await progressBeforeResponse.Content.ReadFromJsonAsync<AthleteDashboardProgressDto>();
        Assert.NotNull(progressBefore);

        var invalidWorkout = TestHelpers.CreateBodyWeightWorkoutSession(
            userId: "integration-user",
            reps: 0);

        var createWorkoutResponse = await client.PostAsJsonAsync("/api/WorkoutSession/new-workout", invalidWorkout);
        Assert.Equal(HttpStatusCode.BadRequest, createWorkoutResponse.StatusCode);
        TestHelpers.LogStatus(_output, "new-workout invalid request", createWorkoutResponse.StatusCode);
        var errorBody = await createWorkoutResponse.Content.ReadAsStringAsync();
        _output.WriteLine($"new-workout invalid response body: {errorBody}");

        var progressAfterResponse = await client.GetAsync("/api/WorkoutSession/dashboard-progress");
        Assert.Equal(HttpStatusCode.OK, progressAfterResponse.StatusCode);
        TestHelpers.LogStatus(_output, "dashboard-progress after invalid request", progressAfterResponse.StatusCode);
        var progressAfter = await progressAfterResponse.Content.ReadFromJsonAsync<AthleteDashboardProgressDto>();
        Assert.NotNull(progressAfter);
        TestHelpers.LogProgress(_output, "progress after invalid request", progressAfter);

        Assert.Equal(progressBefore.Strength.TotalXp, progressAfter.Strength.TotalXp);
        Assert.Equal(progressBefore.Endurance.TotalXp, progressAfter.Endurance.TotalXp);
        Assert.Equal(progressBefore.BodyControl.TotalXp, progressAfter.BodyControl.TotalXp);
    }
}
