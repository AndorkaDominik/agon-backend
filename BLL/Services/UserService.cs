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

public class UserService(
    UserManager<User> userManager, 
    ApplicationDbContext context, 
    IMapper mapper)
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly ApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<User?> GetAuthenticatedUserAsync(ClaimsPrincipal principal)
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

        return user;
    }

    public async Task<ProfileDto?> GetUserProfileAsync(ClaimsPrincipal principal)
    {
        var user = await GetAuthenticatedUserAsync(principal);
        var roles = await _userManager.GetRolesAsync(user);
        var userType = roles.FirstOrDefault() ?? "Unknown";

        if (userType == "Athlete")
        {
            var athlete = await _context.Athletes
                .Include(a => a.AthleteProgress)
                .Include(a => a.Trainer)
                .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(a => a.UserId == user.Id);

            if (athlete == null)
            {
                throw new AthleteNotFoundException();
            }

            return _mapper.Map<ProfileDto>(athlete);
        }
        else if (userType == "Trainer")
        {
            var trainer = await _context.Trainers
                .FirstOrDefaultAsync(t => t.UserId == user.Id);

            if (trainer == null)
            {
                throw new TrainerNotFoundException();
            }

            return _mapper.Map<ProfileDto>(trainer);
        }
        else
        {
            throw new UserTypeNotSupportedException($"User type '{userType}' is not supported.");
        }
    }

    public async Task<ProfileDto?> UpdateUserProfileAsync(ClaimsPrincipal principal, ProfileDto profileDto)
    {
        var validator = new ProfileDtoValidator();
        ValidationResult validationResult = validator.Validate(profileDto);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var user = await GetAuthenticatedUserAsync(principal);
        user.UserName = profileDto.UserName;
        user.Email = profileDto.Email;
        user.NormalizedEmail = profileDto.Email.ToUpper();

        var userUpdateResult = await _userManager.UpdateAsync(user);
        if (!userUpdateResult.Succeeded)
        {
            throw new UserUpdateFailedException("Failed to update user details.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var userType = roles.FirstOrDefault() ?? "Unknown";

        if (userType == "Athlete")
        {
            var athlete = await _context.Athletes
                .FirstOrDefaultAsync(a => a.UserId == user.Id);
            if (athlete == null)
            {
                throw new AthleteNotFoundException();
            }

            _mapper.Map(profileDto, athlete);
            _context.Athletes.Update(athlete);
        }
        else if (userType == "Trainer")
        {
            var trainer = await _context.Trainers
                .FirstOrDefaultAsync(t => t.UserId == user.Id);
            if (trainer == null)
            {
                throw new TrainerNotFoundException();
            }

            _mapper.Map(profileDto, trainer);
            _context.Trainers.Update(trainer);
        }
        else
        {
            throw new UserTypeNotSupportedException($"User type '{userType}' is not supported.");
        }

        await _context.SaveChangesAsync();
        return await GetUserProfileAsync(principal);
    }
}
