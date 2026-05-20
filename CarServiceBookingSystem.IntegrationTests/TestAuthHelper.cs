using CarServiceBookingSystem.Application.Security;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CarServiceBookingSystem.IntegrationTests;

public static class TestAuthHelper
{
    public static string GenerateJwt(
        string userId,
        string email,
        string role = "User",
        IEnumerable<string>? permissions = null)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Role, role)
        };

        if (permissions != null)
        {
            claims.AddRange(
                permissions.Select(permission =>
                    new Claim(CustomClaimTypes.Permission, permission)));
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                "THIS_IS_A_TEST_SECRET_KEY_FOR_INTEGRATION_TESTS_123456789"));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "CarServiceBookingSystem",
            audience: "CarServiceBookingSystemUsers",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}