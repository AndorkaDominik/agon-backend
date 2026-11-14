using AutoMapper;
using BLL.Exceptions;
using BLL.Validators;
using DAL;
using DAL.Entities;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Transfer.Dtos;

namespace BLL.Services;

public class AuthService(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    ApplicationDbContext context,
    IMapper mapper)
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly SignInManager<User> _signInManager = signInManager;
    private readonly ApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var validator = new LoginRequestDtoValidator();
        ValidationResult validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            throw new UserNotFoundException();
        }

        var result = await _signInManager.PasswordSignInAsync(
            user,
            request.Password,
            isPersistent: true,
            lockoutOnFailure: false
        );

        if (!result.Succeeded)
        {
            throw new InvalidCredentialsException("Invalid email or password.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var userType = roles.FirstOrDefault() ?? "Unknown";

        var response = _mapper.Map<AuthResponseDto>(user);
        response.UserType = userType;
        response.ThemeColor = await GetThemeColorAsync(user.Id, userType);

        return response;
    }
    public async Task<AuthResponseDto?> RegisterAsync(RegisterRequestDto request)
    {
        var validator = new RegisterRequestDtoValidator();
        ValidationResult validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var user = _mapper.Map<User>(request);
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            throw new RegistrationFailedException("User registration failed.");
        }

        await _userManager.AddToRoleAsync(user, request.UserType);

        if (request.UserType == "Athlete")
        {
            var athlete = new Athlete
            {
                UserId = user.Id
            };
            _context.Athletes.Add(athlete);
        }
        else if (request.UserType == "Trainer")
        {
            var trainer = new Trainer
            {
                UserId = user.Id,
                Specialization = request.Specialization ?? string.Empty,
                Introduction = request.Introduction ?? string.Empty
            };
            _context.Trainers.Add(trainer);
        }

        await _context.SaveChangesAsync();
        await _signInManager.SignInAsync(user, isPersistent: true);

        var response = _mapper.Map<AuthResponseDto>(user);
        response.UserType = request.UserType;
        response.ThemeColor = await GetThemeColorAsync(user.Id, request.UserType);
        return response;
    }

    public async Task<AuthResponseDto?> GetCurrentUserAsync(ClaimsPrincipal principal)
    {
        if (!principal.Identity?.IsAuthenticated ?? true)
        {
            throw new UserNotAuthenticatedException();
        }

        var user = await _userManager.GetUserAsync(principal);
        if (user == null)
        {
            throw new UserNotFoundException();
        }

        var roles = await _userManager.GetRolesAsync(user);
        var userType = roles.FirstOrDefault() ?? "Unknown";
        var response = _mapper.Map<AuthResponseDto>(user);
        response.UserType = userType;
        response.ThemeColor = await GetThemeColorAsync(user.Id, userType);

        return response;
    }

    private async Task<string?> GetThemeColorAsync(string userId, string userType)
    {
        return userType == "Athlete"
            ? await _context.Athletes.Where(a => a.UserId == userId)
                .Select(a => a.ThemeColor).FirstOrDefaultAsync() ?? "grass"
            : null;
    }

    public async Task LogOutUserAsync()
    {
        try
        {
            await _signInManager.SignOutAsync();
        }
        catch
        {
            throw new LogoutFailedException("Logout failed.");
        }
    }
}
