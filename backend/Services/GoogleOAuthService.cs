using System.Text.Json;
using System.Net.Http;
using BadNews.Models;
using BadNews.Data;
using Microsoft.EntityFrameworkCore;

namespace BadNews.Services;

/// <summary>
/// Service for handling Google OAuth authentication
/// </summary>
public interface IGoogleOAuthService
{
    Task<(bool Success, User? User, string? Error)> AuthenticateWithGoogleTokenAsync(string googleToken, UserRole? role = null);
    Task<(bool Success, User? User, string? Error)> LinkGoogleAccountAsync(Guid userId, string googleToken);
    Task<bool> UnlinkGoogleAccountAsync(Guid userId);
}

public class GoogleOAuthService : IGoogleOAuthService
{
    private readonly BadNewsDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<GoogleOAuthService> _logger;
    private readonly string _googleTokenValidationUrl = "https://www.googleapis.com/oauth2/v3/tokeninfo";

    public GoogleOAuthService(
        BadNewsDbContext context,
        IHttpClientFactory httpClientFactory,
        ILogger<GoogleOAuthService> logger)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    /// <summary>
    /// Authenticate user with Google OAuth token
    /// Creates new user if doesn't exist, updates if exists
    /// </summary>
    public async Task<(bool Success, User? User, string? Error)> AuthenticateWithGoogleTokenAsync(string googleToken, UserRole? role = null)
    {
        try
        {
            // Validate and decode Google token
            var googleInfo = await ValidateGoogleTokenAsync(googleToken);
            if (googleInfo == null)
                return (false, null, "Invalid Google token");

            // Extract user info from Google token
            var googleId = googleInfo["sub"]?.ToString();
            var email = googleInfo["email"]?.ToString();
            var name = googleInfo["name"]?.ToString() ?? "Google User";
            var picture = googleInfo["picture"]?.ToString();

            if (string.IsNullOrEmpty(googleId) || string.IsNullOrEmpty(email))
                return (false, null, "Missing required Google account information");

            // Check if user exists by Google ID
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.GoogleId == googleId);

            if (existingUser != null)
            {
                // Update last login
                existingUser.LastLogin = DateTime.UtcNow;
                existingUser.GoogleEmail = email;
                existingUser.GoogleProfilePictureUrl = picture;
                await _context.SaveChangesAsync();
                return (true, existingUser, null);
            }

            // Check if email already exists (link to existing account)
            var userByEmail = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (userByEmail != null)
            {
                // User exists with this email, link Google account
                userByEmail.GoogleId = googleId;
                userByEmail.GoogleEmail = email;
                userByEmail.GoogleProfilePictureUrl = picture;
                userByEmail.IsGoogleLinked = true;
                userByEmail.LastLogin = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return (true, userByEmail, null);
            }

            // Create new user from Google info
            var names = name.Split(' ', 2);
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                GoogleId = googleId,
                GoogleEmail = email,
                GoogleProfilePictureUrl = picture,
                IsGoogleLinked = true,
                FirstName = names[0],
                LastName = names.Length > 1 ? names[1] : "User",
                Role = role ?? UserRole.Buyer,
                IsActive = true,
                EmailVerified = true, // Google emails are verified
                LastLogin = DateTime.UtcNow,
                PasswordHash = "GOOGLE_OAUTH" // Mark as Google auth
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"New user created via Google OAuth: {email}");
            return (true, newUser, null);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Google OAuth authentication error: {ex.Message}");
            return (false, null, "Authentication failed");
        }
    }

    /// <summary>
    /// Link existing user account with Google
    /// </summary>
    public async Task<(bool Success, User? User, string? Error)> LinkGoogleAccountAsync(Guid userId, string googleToken)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return (false, null, "User not found");

            if (user.IsGoogleLinked)
                return (false, null, "Google account already linked");

            var googleInfo = await ValidateGoogleTokenAsync(googleToken);
            if (googleInfo == null)
                return (false, null, "Invalid Google token");

            var googleId = googleInfo["sub"]?.ToString();
            var email = googleInfo["email"]?.ToString();
            var picture = googleInfo["picture"]?.ToString();

            if (string.IsNullOrEmpty(googleId))
                return (false, null, "Missing Google account ID");

            // Check if Google ID is already in use
            var existingGoogle = await _context.Users
                .FirstOrDefaultAsync(u => u.GoogleId == googleId && u.Id != userId);

            if (existingGoogle != null)
                return (false, null, "This Google account is already linked to another user");

            user.GoogleId = googleId;
            user.GoogleEmail = email;
            user.GoogleProfilePictureUrl = picture;
            user.IsGoogleLinked = true;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Google account linked for user: {userId}");
            return (true, user, null);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Google account linking error: {ex.Message}");
            return (false, null, "Linking failed");
        }
    }

    /// <summary>
    /// Unlink Google account from user
    /// </summary>
    public async Task<bool> UnlinkGoogleAccountAsync(Guid userId)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || !user.IsGoogleLinked)
                return false;

            // Don't unlink if user has no password (can't login)
            if (user.PasswordHash == "GOOGLE_OAUTH")
            {
                _logger.LogWarning($"Cannot unlink Google from user with no password: {userId}");
                return false;
            }

            user.GoogleId = null;
            user.GoogleEmail = null;
            user.GoogleProfilePictureUrl = null;
            user.IsGoogleLinked = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Google account unlinked for user: {userId}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Google account unlinking error: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Validate Google token with Google's token info endpoint
    /// </summary>
    private async Task<Dictionary<string, object>?> ValidateGoogleTokenAsync(string token)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_googleTokenValidationUrl}?access_token={token}");

            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            // Convert JsonElement to Dictionary
            var result = new Dictionary<string, object>();
            foreach (var property in root.EnumerateObject())
            {
                result[property.Name] = property.Value.ToString();
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Google token validation error: {ex.Message}");
            return null;
        }
    }
}
