using BLL.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace API.Handlers;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetails;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IProblemDetailsService problemDetails)
    {
        _logger = logger;
        _problemDetails = problemDetails;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception occurred. TraceId: {TraceId}",
            httpContext.TraceIdentifier);

        var (statusCode, title) = MapException(exception);

        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = GetProblemType(statusCode),
            Instance = httpContext.Request.Path,
            Detail = exception.Message
        };

        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
        problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

        return await _problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });
    }

    private static (int StatusCode, string Title) MapException(Exception exception) => exception switch
    {
        FluentValidation.ValidationException fvEx => (StatusCodes.Status400BadRequest, "Validation failed"),
        ValidationException => (StatusCodes.Status400BadRequest, "Validation failed"),
        RegistrationFailedException => (StatusCodes.Status400BadRequest, "Registration failed"),
        UserUpdateFailedException => (StatusCodes.Status400BadRequest, "User update failed"),
        InvalidCredentialsException => (StatusCodes.Status401Unauthorized, "Invalid credentials"),
        UserNotAuthenticatedException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
        UserNotFoundException => (StatusCodes.Status404NotFound, "User not found"),
        AthleteNotFoundException => (StatusCodes.Status404NotFound, "Athlete not found"),
        TrainerNotFoundException => (StatusCodes.Status404NotFound, "Trainer not found"),
        WorkoutSessionNotFoundException => (StatusCodes.Status404NotFound, "Workout session not found"),
        LogoutFailedException => (StatusCodes.Status400BadRequest, "Logout failed"),
        UserTypeNotSupportedException => (StatusCodes.Status400BadRequest, "User type not supported"),
        InvalidOperationException => (StatusCodes.Status409Conflict, "Operation conflict"),
        KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource not found"),
        AppException appEx => ((int)appEx.StatusCode, appEx.Message),
        ArgumentNullException => (StatusCodes.Status400BadRequest, "Invalid argument provided"),
        ArgumentException => (StatusCodes.Status400BadRequest, "Invalid argument provided"),
        UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
        _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
    };

    private static string GetProblemType(int statusCode) => statusCode switch
    {
        400 => "https://tools.ietf.org/html/rfc9110#section-15.5.1",
        401 => "https://tools.ietf.org/html/rfc9110#section-15.5.2",
        403 => "https://tools.ietf.org/html/rfc9110#section-15.5.4",
        404 => "https://tools.ietf.org/html/rfc9110#section-15.5.5",
        409 => "https://tools.ietf.org/html/rfc9110#section-15.5.10",
        _ => "https://tools.ietf.org/html/rfc9110#section-15.6.1"
    };
}