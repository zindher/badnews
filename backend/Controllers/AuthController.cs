using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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
}

public class UpdateProfileRequest
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
}
