
using API.Mapping;
using AutoMapper;
using API.Handlers;
using BLL.Services;
using BLL.Validators;
using DAL;
using DAL.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Transfer.Dtos;
using API.Hubs;

namespace API;

public class Program
{
    public static void Main(string[] args)
    {
        var AllowFrontendOriginHttp = "http://localhost:3000";
        var AllowFrontendOriginHttps = "https://localhost:3000";

        var builder = WebApplication.CreateBuilder(args);
       
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));
        
        builder.Services.AddIdentityApiEndpoints<User>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        builder.Services.AddSingleton<IEmailSender<User>, DummyEmailSender>();

        // Automapper configuration
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        }, LoggerFactory.Create(builder => { }));

        builder.Services.AddSingleton(sp => mapperConfig.CreateMapper());

        // Add authorization and controllers
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        
        // Real time messaging
        builder.Services.AddSignalR();

        // This wires up the IProblemDetailsService, which ASP.NET Core uses internally to generate RFC-7807 compliant error responses.
        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = ctx =>
            {
                ctx.ProblemDetails.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;
                ctx.ProblemDetails.Extensions["timestamp"] = DateTime.UtcNow;
                ctx.ProblemDetails.Instance = $"{ctx.HttpContext.Request.Method} {ctx.HttpContext.Request.Path}";
            };
        });
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        // Add authorization policies for role-based access control
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("Athlete", policy =>
                policy.RequireRole("Athlete"))
            .AddPolicy("Trainer", policy =>
                policy.RequireRole("Trainer"));

        builder.Services.AddEndpointsApiExplorer();
        // Add Swagger for API documentation
        builder.Services.AddSwaggerGen();
        // Add health checks for monitoring the application's health status
        builder.Services.AddHealthChecks();

        // Add FluentValidation validators for request DTOs
        builder.Services.AddScoped<IValidator<LoginRequestDto>, LoginRequestDtoValidator>();

        // Add bll services to the dependency injection container
        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<WorkoutSessionService>();
        builder.Services.AddScoped<WorkoutXpCalculatorService>();
        builder.Services.AddScoped<AthleteProgressCalculatorService>();
        builder.Services.AddScoped<WorkoutPlanService>();
        builder.Services.AddScoped<AthleteService>();
        builder.Services.AddScoped<TrainerService>();
        builder.Services.AddScoped<DiscussionService>();

        var app = builder.Build();
    
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Add exception handler for global exception handling
        app.UseExceptionHandler();
        // Add status code pages (so even plain 404s / 500s return a body)
        app.UseStatusCodePages();

        app.UseHttpsRedirection();
        app.UseRouting();

        // Configure CORS to allow requests from the frontend application
        app.UseCors(policy =>
        {
            policy.WithOrigins(AllowFrontendOriginHttp, AllowFrontendOriginHttps)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapIdentityApi<User>();
        app.MapControllers();

        // Real time messaging
        app.MapHub<DiscussionHub>("/hubs/discussion");

        app.MapHealthChecks("/health");
        app.MapGet("/", () => "Az API fut!");

        app.Run();
    }
}

public class DummyEmailSender : IEmailSender<User>
{
    public Task SendConfirmationLinkAsync(User user, string email, string confirmationLink)
        => Task.CompletedTask;

    public Task SendPasswordResetLinkAsync(User user, string email, string resetLink)
        => Task.CompletedTask;

    public Task SendPasswordResetCodeAsync(User user, string email, string resetCode)
        => Task.CompletedTask;
}
