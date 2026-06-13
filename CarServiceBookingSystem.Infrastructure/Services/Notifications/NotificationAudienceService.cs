using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Interfaces.INotification;
using CarServiceBookingSystem.Application.Security;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CarServiceBookingSystem.Infrastructure.Services.Notifications;

public class NotificationAudienceService : INotificationAudienceService
{
    private readonly ApplicationDbContext _context;

    public NotificationAudienceService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<string>> GetUserIdsWithPermissionAsync(
        string permission,
        CancellationToken cancellationToken = default)
    {
        var roleIds = await _context.RoleClaims
            .AsNoTracking()
            .Where(x =>
                x.ClaimType == CustomClaimTypes.Permission &&
                x.ClaimValue == permission)
            .Select(x => x.RoleId)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (roleIds.Count == 0)
        {
            return new List<string>();
        }

        return await _context.UserRoles
            .AsNoTracking()
            .Where(x => roleIds.Contains(x.RoleId))
            .Select(x => x.UserId)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<List<string>> GetAdminUserIdsAsync(
        CancellationToken cancellationToken = default)
    {
        return await GetUserIdsWithPermissionAsync(
            Permissions.Notifications.ViewAll,
            cancellationToken);
    }
}