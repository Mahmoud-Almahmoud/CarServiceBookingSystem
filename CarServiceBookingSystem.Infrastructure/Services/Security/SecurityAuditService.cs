using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Domain.Enums;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;

namespace CarServiceBookingSystem.Infrastructure.Services.Security;

public class SecurityAuditService : ISecurityAuditService
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGeoLocationService _geoLocationService;

    public SecurityAuditService(
        ApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor,
        IGeoLocationService geoLocationService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _geoLocationService = geoLocationService;
    }

    public async Task LogAsync(
    string userId,
    SecurityAuditEventType eventType,
    string? details = null)
    {
        var ip = GetIpAddress();
        var geo = await GetGeoLocationAsync(ip);

        var log = new SecurityAuditLog
        {
            UserId = userId,
            EventType = eventType,
            IpAddress = ip,
            Device = GetDevice(),
            Details = details,
            Country = geo.Country,
            City = geo.City
        };

        await _context.SecurityAuditLogs.AddAsync(log);
        await _context.SaveChangesAsync();
    }

    private async Task<GeoLocationResult> GetGeoLocationAsync(string ip)
    {
        return await _geoLocationService.GetLocationAsync(ip);
    }

    private string? GetIpAddress()
    {
        return _httpContextAccessor.HttpContext?
            .Connection.RemoteIpAddress?
            .ToString();
    }

    private string? GetDevice()
    {
        return _httpContextAccessor.HttpContext?
            .Request.Headers.UserAgent
            .ToString();
    }
}