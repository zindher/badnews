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

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Database
Console.WriteLine($"ConnectionString: {settings.Database.ConnectionString}");
try
{
    builder.Services.AddDbContext<BadNewsDbContext>(options =>
        options.UseSqlServer(settings.Database.ConnectionString));
    Console.WriteLine("DbContext registered successfully");
}
catch (Exception ex)
{
    Console.WriteLine($"DbContext error: {ex.Message}");
}

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

Console.WriteLine("Application built successfully");

// Middleware
// app.UseErrorHandling();  // Temporarily disabled
Console.WriteLine("ErrorHandling middleware disabled");

// Enable Swagger for all environments
app.UseSwagger();
Console.WriteLine("Swagger enabled");
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BadNews API V1");
    c.RoutePrefix = "swagger"; // Swagger available at /swagger
});
Console.WriteLine("SwaggerUI configured");

app.UseCors("AllowFrontend");
Console.WriteLine("CORS configured");
app.UseAuthentication();
Console.WriteLine("Authentication configured");
app.UseAuthorization();
Console.WriteLine("Authorization configured");
app.MapControllers();
Console.WriteLine("Controllers mapped");

Console.WriteLine("Starting application...");
try
{
    await app.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Application error: {ex}");
}
Console.WriteLine("Application stopped");
