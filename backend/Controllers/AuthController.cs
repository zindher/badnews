using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using BadNews.Services;
using BadNews.Data;
using BadNews.DTOs;

namespace BadNews.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly BadNewsDbContext _dbContext;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, BadNewsDbContext dbContext, ILogger<AuthController> logger)
    {
        _authService = authService;
        _dbContext = dbContext;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _authService.RegisterAsync(request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _authService.LoginAsync(request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging in user");
            return Unauthorized(new { success = false, message = "Invalid credentials" });
        }
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
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
                    user.Role
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting profile");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
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

            return Ok(new { success = true, message = "Profile updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request, [FromServices] IGoogleOAuthService googleOAuthService, [FromServices] IJwtService jwtService)
    {
        if (!ModelState.IsValid || string.IsNullOrEmpty(request.GoogleToken))
            return BadRequest(new { success = false, message = "Google token is required" });

        try
        {
            // Authenticate with Google
            var (success, user, error) = await googleOAuthService.AuthenticateWithGoogleTokenAsync(request.GoogleToken, request.Role);

            if (!success || user == null)
                return Unauthorized(new { success = false, message = error ?? "Google authentication failed" });

            // Generate JWT token
            var token = jwtService.GenerateToken(user);

            return Ok(new
            {
                success = true,
                data = new
                {
                    token,
                    userId = user.Id,
                    email = user.Email,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    role = user.Role.ToString(),
                    profilePicture = user.GoogleProfilePictureUrl
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error with Google login");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    [HttpPost("link-google")]
    [Authorize]
    public async Task<IActionResult> LinkGoogle([FromBody] GoogleLoginRequest request, [FromServices] IGoogleOAuthService googleOAuthService)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userId, out var parsedUserId))
                return Unauthorized();

            var (success, user, error) = await googleOAuthService.LinkGoogleAccountAsync(parsedUserId, request.GoogleToken);

            if (!success)
                return BadRequest(new { success = false, message = error });

            return Ok(new { success = true, message = "Google account linked successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error linking Google account");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
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
                return BadRequest(new { success = false, message = "Failed to update password" });

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

    [Authorize]
    [HttpPut("change-email")]
    public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailRequest request)
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

public class GoogleLoginRequest
{
    public string GoogleToken { get; set; } = null!;
    public BadNews.Models.UserRole? Role { get; set; }
}

public class UpdateProfileRequest
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
}

public class ChangePasswordRequest
{
    [Required]
    public string CurrentPassword { get; set; } = null!;

    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string NewPassword { get; set; } = null!;
}

public class ChangeEmailRequest
{
    [Required]
    [EmailAddress]
    public string NewEmail { get; set; } = null!;
}
