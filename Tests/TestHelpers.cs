using DAL.Entities;
using DtoExerciseType = Transfer.Dtos.ExerciseType;
using EntityExerciseType = DAL.Entities.ExerciseType;
using System.Net;
using Transfer.Dtos;
using Xunit.Abstractions;

namespace Tests;

public class TestHelpers
{
    public static WorkoutSession CreateSession(params SessionExercise[] exercises) => new()
    {
        UserId = "user-1",
        AthleteId = 1,
        WorkoutName = "test-session",
        Date = DateTime.UnixEpoch,
        Exercises = exercises.ToList()
    };

    public static SessionExercise Strength(double weight, int reps) => new()
    {
        ExerciseType = EntityExerciseType.Strength,
        ExerciseName = "Strength",
        Weight = weight,
        Reps = reps
    };

    public static SessionExercise Cardio(double durationMinutes, double distanceKm) => new()
    {
        ExerciseType = EntityExerciseType.Cardio,
        ExerciseName = "Cardio",
        Duration = durationMinutes,
        Distance = distanceKm
    };

    public static SessionExercise Bodyweight(int reps) => new()
    {
        ExerciseType = EntityExerciseType.Bodyweight,
        ExerciseName = "Bodyweight",
        Reps = reps
    };

    public static RegisterRequestDto CreateRegisterRequest(
        string userName = "integration-athlete",
        string? email = null,
        string password = "StrongPass1",
        string userType = "Athlete") => new()
    {
        UserType = userType,
        UserName = userName,
        Email = email ?? $"integration-athlete-{Guid.NewGuid():N}@gmail.com",
        Password = password,
        PasswordConfirm = password
    };

    public static WorkoutSessionDto CreateStrengthWorkoutSession(
        string userId,
        string workoutName = "Strength workout",
        string exerciseName = "Bench Press",
        double weight = 100,
        int reps = 5) => new()
    {
        UserId = userId,
        WorkoutName = workoutName,
        Exercises =
        [
            new SessionExerciseDto
            {
                ExerciseName = exerciseName,
                ExerciseType = DtoExerciseType.Strength,
                Weight = weight,
                Reps = reps
            }
        ]
    };

    public static WorkoutSessionDto CreateBodyWeightWorkoutSession(
       string userId,
       string workoutName = "Bodyweight workout",
       string exerciseName = "Push Ups",
       int reps = 5) => new()
       {
           UserId = userId,
           WorkoutName = workoutName,
           Exercises =
       [
           new SessionExerciseDto
            {
                ExerciseName = exerciseName,
                ExerciseType = DtoExerciseType.Bodyweight,
                Reps = reps
            }
       ]
       };

    public static WorkoutXpRewardDto CreateExpectedStrengthWorkoutReward() => new()
    {
        StrengthXp = 22,
        EnduranceXp = 0,
        BodyControlXp = 0
    };

    public static void LogStatus(ITestOutputHelper output, string label, HttpStatusCode statusCode)
    {
        output.WriteLine($"{label}: {(int)statusCode} ({statusCode})");
    }

    public static void LogProgress(ITestOutputHelper output, string label, AthleteDashboardProgressDto progress)
    {
        output.WriteLine($"{label}: Strength={progress.Strength.TotalXp}, Endurance={progress.Endurance.TotalXp}, BodyControl={progress.BodyControl.TotalXp}");
    }

    public static object ToProgressSnapshot(AthleteDashboardProgressDto progress) => new
    {
        Strength = progress.Strength.TotalXp,
        Endurance = progress.Endurance.TotalXp,
        BodyControl = progress.BodyControl.TotalXp
    };
}