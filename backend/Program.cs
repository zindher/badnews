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
builder.Services.AddScoped<IMercadoPagoService, MercadoPagoServiceImpl>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ITimezoneService, TimezoneService>();

// Database
builder.Services.AddDbContext<BadNewsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BadNews") ?? 
        "Server=(localdb)\\mssqllocaldb;Database=BadNews;Trusted_Connection=true;"));

// Logging
builder.Services.AddLogging();

var app = builder.Build();

// Middleware
app.UseErrorHandling();

// Enable Swagger for all environments
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BadNews API V1");
    c.RoutePrefix = string.Empty; // Make Swagger UI the root
});

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
