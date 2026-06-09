using CarServiceBookingSystem.Application.Common;

namespace CarServiceBookingSystem.Application.Interfaces.IAuth;

public interface ITokenService
{
    Task<string> CreateAccessTokenAsync(AuthUser user);
    string GenerateRefreshToken();
}