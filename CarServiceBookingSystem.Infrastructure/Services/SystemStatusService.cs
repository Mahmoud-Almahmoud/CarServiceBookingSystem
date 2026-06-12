using CarServiceBookingSystem.Application.DTOs;
using CarServiceBookingSystem.Application.DTOs.System;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Reflection;

public class SystemStatusService : ISystemStatusService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public SystemStatusService(
        ApplicationDbContext context,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        _context = context;
        _configuration = configuration;
        _environment = environment;
    }

    public async Task<SystemStatusResponse> GetStatusAsync()
    {
        var databaseReachable = await _context.Database.CanConnectAsync();

        var stripeSecretKey = _configuration["Stripe:SecretKey"];
        var stripeWebhookSecret = _configuration["Stripe:WebhookSecret"];

        var emailHost = _configuration["Email:SmtpServer"];
        var emailUsername = _configuration["Email:Username"];

        var env = _environment.EnvironmentName;
        var geoLitePath = "";

        if (_environment.IsDevelopment() || _environment.IsEnvironment("Testing"))
            geoLitePath = Path.Combine(
                _environment.ContentRootPath,
                "GeoLite2-City.mmdb");
        else if(_environment.IsProduction())
            geoLitePath = Path.Combine(
                _environment.ContentRootPath,
                "App_Data",
                "GeoLite2-City.mmdb");

        return new SystemStatusResponse
        {
            DatabaseReachable = databaseReachable,
            StripeConfigured =
                !string.IsNullOrWhiteSpace(stripeSecretKey) &&
                !string.IsNullOrWhiteSpace(stripeWebhookSecret),

            EmailConfigured =
                !string.IsNullOrWhiteSpace(emailHost) &&
                !string.IsNullOrWhiteSpace(emailUsername),

            GeoLiteDbExists = File.Exists(geoLitePath),

            Environment = env,
            ServerTimeUtc = DateTime.UtcNow.ToString("O"),
            AppVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString()
        };
    }

    public SystemConfigurationResponse GetConfiguration()
    {
        return new SystemConfigurationResponse
        {
            Environment = _environment.EnvironmentName,

            JwtConfigured =
                !string.IsNullOrWhiteSpace(_configuration["Jwt:Key"]) &&
                !string.IsNullOrWhiteSpace(_configuration["Jwt:Issuer"]) &&
                !string.IsNullOrWhiteSpace(_configuration["Jwt:Audience"]),

            RefreshTokensEnabled = true,
            TwoFactorEnabled = true,
            ApiKeysEnabled = true,

            StripeConfigured =
                !string.IsNullOrWhiteSpace(_configuration["Stripe:SecretKey"]) &&
                !string.IsNullOrWhiteSpace(_configuration["Stripe:WebhookSecret"]),

            EmailConfigured =
                !string.IsNullOrWhiteSpace(_configuration["Email:SmtpServer"]) &&
                !string.IsNullOrWhiteSpace(_configuration["Email:Username"]),

            SwaggerProtected =
                _environment.IsDevelopment() ||
                _configuration.GetValue<bool>("SwaggerAuth:Required"),

            HangfireProtected = true,

            RateLimitingEnabled = true,
            SecurityHeadersEnabled = true
        };
    }
}