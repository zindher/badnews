using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using BadNews.Models;
using BadNews.Data;
using BadNews.DTOs;

namespace BadNews.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<bool> ValidateUserAsync(string email, string password);
    string GenerateJwtToken(Models.User user);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}

public class AuthService : IAuthService
{
    private readonly BadNewsDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(BadNewsDbContext dbContext, IConfiguration configuration, ILogger<AuthService> logger)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Check if user already exists
        var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (existingUser != null)
            throw new InvalidOperationException("User already exists with this email");

        // Validate T&C acceptance
        if (!request.TermsAcceptedAt.HasValue)
            throw new InvalidOperationException("You must accept the Terms and Conditions to register");

        var user = new Models.User
        {
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PasswordHash = HashPassword(request.Password),
            Role = request.Role == "Messenger" ? Models.UserRole.Messenger : Models.UserRole.Buyer,
            IsActive = true,
            TermsAcceptedAt = request.TermsAcceptedAt,
            TermsAcceptedVersion = "1.0"
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        // Create messenger profile if needed
        if (user.Role == Models.UserRole.Messenger)
        {
            var messenger = new Models.Messenger { UserId = user.Id };
            _dbContext.Messengers.Add(messenger);
            await _dbContext.SaveChangesAsync();
        }

        _logger.LogInformation($"User registered: {user.Email} - Terms accepted at {request.TermsAcceptedAt}");

        return new AuthResponse
        {
            UserId = user.Id.ToString(),
            Email = user.Email,
            Token = GenerateJwtToken(user),
            Role = user.Role.ToString()
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password");

        _logger.LogInformation($"User logged in: {user.Email}");

        return new AuthResponse
        {
            UserId = user.Id.ToString(),
            Email = user.Email,
            Token = GenerateJwtToken(user),
            Role = user.Role.ToString()
        };
    }

    public async Task<bool> ValidateUserAsync(string email, string password)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        return user != null && VerifyPassword(password, user.PasswordHash);
    }

    public string GenerateJwtToken(Models.User user)
    {
        var secret = _configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
        var issuer = _configuration["Jwt:Issuer"] ?? "BadNews";
        var audience = _configuration["Jwt:Audience"] ?? "BadNewsUsers";
        var expirationMinutes = int.TryParse(_configuration["Jwt:ExpirationMinutes"], out var minutes) ? minutes : 60;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("PhoneNumber", user.PhoneNumber)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        var encodedToken = tokenHandler.WriteToken(token);

        return encodedToken;
    }

    public string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    public bool VerifyPassword(string password, string hash)
    {
        var hashOfInput = HashPassword(password);
        return hashOfInput == hash;
    }
}
