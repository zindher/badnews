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

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

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
builder.Services.AddScoped<IMessengerService, MessengerService>();
builder.Services.AddScoped<ITwilioService, TwilioServiceImpl>();
builder.Services.AddScoped<IMercadoPagoService, MercadoPagoServiceImpl>();
builder.Services.AddScoped<ISendGridService, SendGridServiceImpl>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ICallRetryJob, CallRetryJob>();
builder.Services.AddScoped<ITimezoneService, TimezoneService>();

// Background jobs
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(settings.Database.ConnectionString));
builder.Services.AddHangfireServer();

// Logging
builder.Services.AddLogging();

var app = builder.Build();

// Middleware
app.UseErrorHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Hangfire Dashboard
app.UseHangfireDashboard("/hangfire");

app.Run();
