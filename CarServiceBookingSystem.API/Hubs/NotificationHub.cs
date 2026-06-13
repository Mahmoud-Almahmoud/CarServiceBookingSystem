using CarServiceBookingSystem.Application.Constants;
using CarServiceBookingSystem.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CarServiceBookingSystem.API.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        if (HasPermission(Permissions.Notifications.ViewAll))
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                NotificationGroups.Admins);
        }

        if (HasPermission(Permissions.Technicians.ViewAssignedBookings))
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                NotificationGroups.Technicians);
        }

        var branchIdClaim = Context.User?.FindFirst("branchId")?.Value;

        if (int.TryParse(branchIdClaim, out var branchId))
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                NotificationGroups.BranchAdmins(branchId));
        }


        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

    private bool HasPermission(string permission)
    {
        return Context.User?.Claims.Any(x =>
            x.Type == CustomClaimTypes.Permission &&
            x.Value == permission) == true;
    }
}