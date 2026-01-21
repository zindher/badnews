using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BadNews.Services;

public interface IJwtService
{
    string GenerateToken(Models.User user);
    ClaimsPrincipal? ValidateToken(string token);
    string RefreshToken(string token);
}

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtService> _logger;

    public JwtService(IConfiguration configuration, ILogger<JwtService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public string GenerateToken(Models.User user)
    {
        var secret = _configuration["Jwt:Secret"] 
            ?? throw new InvalidOperationException("JWT Secret not configured");
        var issuer = _configuration["Jwt:Issuer"] ?? "BadNews";
        var audience = _configuration["Jwt:Audience"] ?? "BadNewsUsers";
        var expirationMinutes = int.TryParse(_configuration["Jwt:ExpirationMinutes"], out var minutes) 
            ? minutes : 60;

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

        _logger.LogInformation($"JWT token generated for user {user.Email}");

        return encodedToken;
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var secret = _configuration["Jwt:Secret"]
                ?? throw new InvalidOperationException("JWT Secret not configured");
            var issuer = _configuration["Jwt:Issuer"] ?? "BadNews";
            var audience = _configuration["Jwt:Audience"] ?? "BadNewsUsers";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            _logger.LogInformation($"Token validated successfully");
            return principal;
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Token validation failed: {ex.Message}");
            return null;
        }
    }

    public string RefreshToken(string token)
    {
        var principal = ValidateToken(token);
        if (principal == null)
            throw new InvalidOperationException("Invalid token");

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            throw new InvalidOperationException("User ID not found in token");

        // Create new token with same claims
        var secret = _configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("JWT Secret not configured");
        var issuer = _configuration["Jwt:Issuer"] ?? "BadNews";
        var audience = _configuration["Jwt:Audience"] ?? "BadNewsUsers";
        var expirationMinutes = int.TryParse(_configuration["Jwt:ExpirationMinutes"], out var minutes)
            ? minutes : 60;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var newToken = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: principal.Claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        var encodedToken = tokenHandler.WriteToken(newToken);

        _logger.LogInformation($"Token refreshed for user {userIdClaim.Value}");

        return encodedToken;
    }
}
