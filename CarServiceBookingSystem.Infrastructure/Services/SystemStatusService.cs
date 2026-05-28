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

        var stripeSecretKey = _configuration["StripeSettings:SecretKey"];
        var stripeWebhookSecret = _configuration["StripeSettings:WebhookSecret"];

        var emailHost = _configuration["EmailSettings:Host"];
        var emailUsername = _configuration["EmailSettings:FromEmail"];

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
}