using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BadNews.Models;
using BadNews.Data;
using Microsoft.EntityFrameworkCore;

namespace BadNews.Services;

/// <summary>
/// Service for handling Apple Sign-In authentication
/// </summary>
public interface IAppleSignInService
{
    Task<(bool Success, User? User, string? Error)> AuthenticateWithAppleTokenAsync(string identityToken, string? firstName, string? lastName, UserRole? role = null);
}

public class AppleSignInService : IAppleSignInService
{
    private readonly BadNewsDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AppleSignInService> _logger;
    private readonly string? _appleClientId;
    private const string ApplePublicKeysUrl = "https://appleid.apple.com/auth/keys";
    private const string AppleIssuer = "https://appleid.apple.com";

    public AppleSignInService(
        BadNewsDbContext context,
        IHttpClientFactory httpClientFactory,
        ILogger<AppleSignInService> logger,
        IConfiguration configuration)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _appleClientId = configuration["Apple:ClientId"];
    }

    /// <summary>
    /// Authenticate user with Apple identity token.
    /// Creates new user if doesn't exist, updates if exists.
    /// </summary>
    public async Task<(bool Success, User? User, string? Error)> AuthenticateWithAppleTokenAsync(
        string identityToken, string? firstName, string? lastName, UserRole? role = null)
    {
        try
        {
            var claims = await ValidateAppleTokenAsync(identityToken);
            if (claims == null)
                return (false, null, "Invalid Apple identity token");

            var appleId = claims.TryGetValue("sub", out var sub) ? sub : null;
            var email = claims.TryGetValue("email", out var em) ? em : null;

            if (string.IsNullOrEmpty(appleId))
                return (false, null, "Missing Apple user ID");

            // Check if user exists by Apple ID
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.AppleId == appleId);

            if (existingUser != null)
            {
                existingUser.LastLogin = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return (true, existingUser, null);
            }

            // Check if email already exists (link to existing account)
            if (!string.IsNullOrEmpty(email))
            {
                var userByEmail = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (userByEmail != null)
                {
                    userByEmail.AppleId = appleId;
                    userByEmail.IsAppleLinked = true;
                    userByEmail.LastLogin = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    return (true, userByEmail, null);
                }
            }

            // Apple only provides name on first sign-in; use fallback if not provided
            var resolvedEmail = email ?? $"{appleId}@privaterelay.appleid.com";
            var resolvedFirstName = string.IsNullOrEmpty(firstName) ? "Apple" : firstName;
            var resolvedLastName = string.IsNullOrEmpty(lastName) ? "User" : lastName;

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Email = resolvedEmail,
                AppleId = appleId,
                IsAppleLinked = true,
                FirstName = resolvedFirstName,
                LastName = resolvedLastName,
                Role = role ?? UserRole.Buyer,
                IsActive = true,
                EmailVerified = true,
                LastLogin = DateTime.UtcNow,
                PasswordHash = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            _logger.LogInformation("New user created via Apple Sign-In: {AppleId}", appleId);
            return (true, newUser, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apple Sign-In authentication error");
            return (false, null, "Authentication failed");
        }
    }

    /// <summary>
    /// Validate Apple identity token and return claims dictionary.
    /// </summary>
    private async Task<Dictionary<string, string>?> ValidateAppleTokenAsync(string identityToken)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(identityToken))
                return null;

            // Fetch Apple's public keys
            var client = _httpClientFactory.CreateClient();
            var keysResponse = await client.GetStringAsync(ApplePublicKeysUrl);
            var jwks = new JsonWebKeySet(keysResponse);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = AppleIssuer,
                ValidateAudience = !string.IsNullOrEmpty(_appleClientId),
                ValidAudience = _appleClientId,
                ValidateLifetime = true,
                IssuerSigningKeys = jwks.Keys,
                ValidateIssuerSigningKey = true,
            };

            var principal = handler.ValidateToken(identityToken, validationParameters, out _);

            var claims = new Dictionary<string, string>();
            foreach (var claim in principal.Claims)
            {
                claims[claim.Type] = claim.Value;
            }

            // Map standard JWT sub to "sub" key
            if (!claims.ContainsKey("sub") && claims.TryGetValue(System.Security.Claims.ClaimTypes.NameIdentifier, out var nameId))
                claims["sub"] = nameId;

            return claims;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apple token validation error");
            return null;
        }
    }
}
