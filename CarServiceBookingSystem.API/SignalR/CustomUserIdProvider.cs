using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace CarServiceBookingSystem.API.SignalR;

public class CustomUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? connection.User?.FindFirstValue("sub")
            ?? connection.User?.FindFirstValue("userId");
    }
}