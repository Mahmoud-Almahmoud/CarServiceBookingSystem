using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.Interfaces.IAuth;
using CarServiceBookingSystem.Application.Options;
using CarServiceBookingSystem.Application.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CarServiceBookingSystem.Infrastructure.Services.Authentication;

public class TokenService : ITokenService
{
    private readonly JwtOptions _jwtSettings;

    public TokenService(IOptions<JwtOptions> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<string> CreateAccessTokenAsync(AuthUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName)
        };
        if (user.SessionId.HasValue)
        {
            claims.Add(new Claim("session_id", user.SessionId.Value.ToString()));
        }

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        foreach (var permission in user.Permissions)
        {
            claims.Add(new Claim(CustomClaimTypes.Permission, permission));
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.Key));

        var creds = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        return await Task.FromResult(
            new JwtSecurityTokenHandler().WriteToken(token));
    }

    public string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString();
    }
}