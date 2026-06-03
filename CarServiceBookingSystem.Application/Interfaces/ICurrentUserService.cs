namespace CarServiceBookingSystem.Application.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? FullName { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
}