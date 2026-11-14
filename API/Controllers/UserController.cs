using BLL.Exceptions;
using BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transfer.Dtos;
namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController(UserService userService) : ControllerBase
{
    private readonly UserService _userService = userService;

    [HttpGet("profile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProfileDto>> GetUserProfile()
    {
        var profileDto = await _userService.GetUserProfileAsync(User);
        if (profileDto == null)
        {
            return NotFound("Profile not found");
        }

        return Ok(profileDto);
    }

    [HttpPut("profile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProfileDto>> UpdateProfile([FromBody] ProfileDto profileDto)
    {
        try
        {
            var updatedProfile = await _userService.UpdateUserProfileAsync(User, profileDto);
            if (updatedProfile == null)
            {
                return BadRequest("Failed to update profile");
            }

            return Ok(updatedProfile);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new
            {
                message = "Validation failed.",
                errors = ex.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage })
            });
        }
    }
}
