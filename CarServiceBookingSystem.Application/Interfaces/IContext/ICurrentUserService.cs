namespace CarServiceBookingSystem.Application.Interfaces.IContext;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? FullName { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
}