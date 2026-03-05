using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BadNews.Services;
using BadNews.Data;

namespace BadNews.Controllers;

/// <summary>
/// Handles /api/users/me routes used by the frontend authService.
/// Delegates to the same logic as AuthController where applicable.
/// </summary>
[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly BadNewsDbContext _dbContext;
    private readonly IAuthService _authService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(BadNewsDbContext dbContext, IAuthService authService, ILogger<UsersController> logger)
    {
        _dbContext = dbContext;
        _authService = authService;
        _logger = logger;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userId, out var parsedUserId))
                return Unauthorized();

            var user = await _dbContext.Users.FindAsync(parsedUserId);
            if (user == null)
                return NotFound();

            return Ok(new
            {
                success = true,
                data = new
                {
                    user.Id,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.PhoneNumber,
                    user.Role,
                    user.IsGoogleLinked,
                    user.GoogleProfilePictureUrl
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user profile");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateMeRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userId, out var parsedUserId))
                return Unauthorized();

            var user = await _dbContext.Users.FindAsync(parsedUserId);
            if (user == null)
                return NotFound();

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;
            user.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                data = new
                {
                    user.Id,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.PhoneNumber,
                    user.Role
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    [HttpPut("me/password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangeMyPasswordRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userId, out var parsedUserId))
                return Unauthorized();

            var user = await _dbContext.Users.FindAsync(parsedUserId);
            if (user == null)
                return NotFound();

            if (!_authService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
                return BadRequest(new { success = false, message = "Current password is incorrect" });

            user.PasswordHash = _authService.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return Ok(new { success = true, message = "Password changed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    [HttpPut("me/email")]
    public async Task<IActionResult> ChangeEmail([FromBody] ChangeMyEmailRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userId, out var parsedUserId))
                return Unauthorized();

            var existing = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.NewEmail);
            if (existing != null)
                return BadRequest(new { success = false, message = "Email already in use" });

            var user = await _dbContext.Users.FindAsync(parsedUserId);
            if (user == null)
                return NotFound();

            user.Email = request.NewEmail;
            user.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return Ok(new { success = true, message = "Email changed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing email");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }
}

public class UpdateMeRequest
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? PhoneNumber { get; set; }
}

public class ChangeMyPasswordRequest
{
    public string CurrentPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}

public class ChangeMyEmailRequest
{
    public string NewEmail { get; set; } = null!;
}
