using CarServiceBookingSystem.Application.Common;

namespace CarServiceBookingSystem.Application.Interfaces;

public interface ITokenService
{
    Task<string> CreateAccessTokenAsync(AuthUser user);
    string GenerateRefreshToken();
}