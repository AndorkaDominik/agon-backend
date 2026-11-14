using Bogus;
using DAL;
using DAL.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Respawn;
using Respawn.Graph;
using System.Data.Common;
using System.Net;
using Testcontainers.MsSql;
using Transfer.Dtos;

namespace Tests.IntegrationTests.Infrastructure;

public sealed class TestFixture : IAsyncLifetime
{
    private const int SyntheticWorkoutSeed = 2026;

    public readonly RegisterRequestDto _defaultAthleteRequest;

    private readonly MsSqlContainer _sqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    private SqlConnection _connection = null!;
    private Respawner _respawner = null!;

    public TestFactory Factory { get; private set; } = null!;
    public HttpClient Client { get; private set; } = null!;

    public TestFixture()
    {
        _defaultAthleteRequest = new RegisterRequestDto
        {
            UserType = "Athlete",
            UserName = "integration-athlete",
            Email = "integration-athlete@gmail.com",
            Password = "Asd123!",
            PasswordConfirm = "Asd123!"
        };
    }

    public async Task InitializeAsync()
    {
        await _sqlContainer.StartAsync();

        var connectionString = BuildTestDatabaseConnectionString(_sqlContainer.GetConnectionString());
        Factory = new TestFactory(connectionString);

        Client = Factory.CreateClient();

        using (var scope = Factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();
        }

        _connection = new SqlConnection(connectionString);
        await _connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.SqlServer,
            SchemasToInclude = ["dbo"],
            TablesToIgnore =
            [
                new Table("__EFMigrationsHistory"),
                new Table("AspNetRoles")
            ]
        });

        await CleanupAsync();
    }

    public async Task CleanupAsync()
    {
        await _respawner.ResetAsync(_connection);
        await EnsureSyntheticDataForExistingTestsAsync();
    }

    public async Task<HttpClient> CreateAuthenticatedAthleteClientAsync()
    {
        await EnsureSyntheticDataForExistingTestsAsync();

        var client = Factory.CreateClient();

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginRequestDto
        {
            Email = _defaultAthleteRequest.Email,
            Password = _defaultAthleteRequest.Password
        });

        if (loginResponse.StatusCode != HttpStatusCode.OK) throw new InvalidOperationException("Failed to sign in default integration athlete.");

        await EnsureLoggedWorkoutsAsync(client);

        return client;
    }

    public async Task DisposeAsync()
    {
        Client.Dispose();
        await _connection.DisposeAsync();
        await _sqlContainer.DisposeAsync();
        await Factory.DisposeAsync();
    }

    private async Task EnsureSyntheticDataForExistingTestsAsync()
    {
        await EnsureUserRegisteredAsync(_defaultAthleteRequest, "default integration athlete");

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var defaultUser = await dbContext.Users.FirstAsync(u => u.Email == _defaultAthleteRequest.Email);
        var defaultAthlete = await EnsureAthleteAsync(dbContext, defaultUser.Id);

        defaultAthlete.TrainerId = null;
        defaultAthlete.AthleteProgress.StrengthXp = 0;
        defaultAthlete.AthleteProgress.EnduranceXp = 0;
        defaultAthlete.AthleteProgress.BodyControlXp = 0;

        await dbContext.SaveChangesAsync();
    }

    private async Task EnsureUserRegisteredAsync(RegisterRequestDto registerRequest, string userDescription)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var exists = await dbContext.Users.AnyAsync(u => u.Email == registerRequest.Email);
        if (exists) return;

        var registerResponse = await Client.PostAsJsonAsync("/api/auth/register", registerRequest);
        if (registerResponse.StatusCode != HttpStatusCode.Created) throw new InvalidOperationException($"Failed to create {userDescription}.");
    }

    private static async Task<Athlete> EnsureAthleteAsync(ApplicationDbContext dbContext, string userId)
    {
        var athlete = await dbContext.Athletes
            .Include(a => a.AthleteProgress)
            .FirstOrDefaultAsync(a => a.UserId == userId);

        if (athlete == null)
        {
            athlete = new Athlete
            {
                UserId = userId,
                AthleteProgress = new AthleteProgress()
            };
            dbContext.Athletes.Add(athlete);
        }
        else if (athlete.AthleteProgress == null)
        {
            athlete.AthleteProgress = new AthleteProgress();
        }

        return athlete;
    }

    private async Task EnsureLoggedWorkoutsAsync(HttpClient authenticatedClient)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var defaultUser = await dbContext.Users.FirstAsync(u => u.Email == _defaultAthleteRequest.Email);
        var defaultAthlete = await EnsureAthleteAsync(dbContext, defaultUser.Id);

        var existingWorkoutCount = await dbContext.WorkoutSessions.CountAsync(ws => ws.AthleteId == defaultAthlete.Id);
        if (existingWorkoutCount >= 2) return;

        Randomizer.Seed = new Random(SyntheticWorkoutSeed);
        var faker = new Faker("en");

        var workoutsToCreate = new List<WorkoutSessionDto>
        {
            new()
            {
                UserId = "integration-user",
                WorkoutName = faker.PickRandom("Seed strength A", "Seed strength B", "Seed strength C"),
                Notes = faker.Lorem.Sentence(4),
                Exercises =
                [
                    new SessionExerciseDto
                    {
                        ExerciseName = faker.PickRandom("Bench Press", "Squat", "Overhead Press"),
                        ExerciseType = Transfer.Dtos.ExerciseType.Strength,
                        Weight = faker.Random.Double(70, 90),
                        Reps = faker.Random.Int(5, 8)
                    }
                ]
            },
            new()
            {
                UserId = "integration-user",
                WorkoutName = faker.PickRandom("Seed cardio A", "Seed cardio B", "Seed cardio C"),
                Notes = faker.Lorem.Sentence(4),
                Exercises =
                [
                    new SessionExerciseDto
                    {
                        ExerciseName = faker.PickRandom("Outdoor Run", "Bike", "Treadmill"),
                        ExerciseType = Transfer.Dtos.ExerciseType.Cardio,
                        Duration = faker.Random.Double(25, 35),
                        Distance = faker.Random.Double(4, 7)
                    }
                ]
            }
        };

        foreach (var workout in workoutsToCreate)
        {
            var createResponse = await authenticatedClient.PostAsJsonAsync("/api/WorkoutSession/new-workout", workout);
            if (createResponse.StatusCode != HttpStatusCode.Created)
            {
                throw new InvalidOperationException("Failed to create deterministic seeded workout.");
            }
        }
    }

    private static string BuildTestDatabaseConnectionString(string containerConnectionString)
    {
        var builder = new DbConnectionStringBuilder { ConnectionString = containerConnectionString };

        builder["Database"] = "AgonIntegrationTests";

        return builder.ConnectionString;
    }
}
