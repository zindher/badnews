using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BadNews.Data;
using BadNews.Configurations;
using BadNews.Services;
using BadNews.Middleware;
using FluentValidation;
using BadNews.Validators;

var builder = WebApplication.CreateBuilder(args);

// Load configuration
var settings = new AppSettings();
builder.Configuration.Bind(settings);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS - restrict to known origins in production
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:5173" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

// Database
builder.Services.AddDbContext<BadNewsDbContext>(options =>
    options.UseSqlServer(settings.Database.ConnectionString));

// Configuration
builder.Services.Configure<AppSettings>(builder.Configuration);

// JWT Authentication
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
var key = Encoding.ASCII.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Application Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ITwilioService, TwilioServiceImpl>();
builder.Services.AddHttpClient<IMercadoPagoService, MercadoPagoServiceImpl>();
builder.Services.AddHttpClient<IGoogleOAuthService, GoogleOAuthService>();

// Email Service Factory
builder.Services.AddScoped<IEmailService>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var apiKey = config["SendGrid:ApiKey"] ?? "test-key";
    var fromEmail = config["SendGrid:FromEmail"] ?? "noreply@badnews.com";
    var fromName = config["SendGrid:FromName"] ?? "BadNews";
    return new EmailService(apiKey, fromEmail, fromName);
});

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ITimezoneService, TimezoneService>();

// Logging
builder.Services.AddLogging();

var app = builder.Build();

// Error handling middleware (global exception handler)
app.UseErrorHandling();

// Swagger only in non-production environments
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BadNews API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
