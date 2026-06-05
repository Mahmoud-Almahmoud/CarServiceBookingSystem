using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.DTOs.Auth;
using CarServiceBookingSystem.Infrastructure.Identity;
using CarServiceBookingSystem.Infrastructure.Persistence;
using CarServiceBookingSystem.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace CarServiceBookingSystem.UnitTests.TestHelpers;

public static class AuthServiceTestFactory
{
    public static AuthService Create(
        ApplicationDbContext context,
        Mock<UserManager<ApplicationUser>> userManagerMock,
        Mock<ITokenService>? tokenServiceMock = null,
        HttpContextAccessor? httpContextAccessor = null,
        Mock<IBackgroundJobService>? backgroundJobServiceMock = null,
        Mock<ISecurityAuditService>? securityAuditServiceMock = null,
        Mock<IQrCodeService>? qrCodeServiceMock = null,
        Mock<IGeoLocationService>? geoLocationServiceMock = null)
    {
        var roleManagerMock = new Mock<RoleManager<IdentityRole>>(
    new Mock<IRoleStore<IdentityRole>>().Object,
    null!,
    null!,
    null!,
    null!);
        tokenServiceMock ??= new Mock<ITokenService>();
        httpContextAccessor ??= new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext()
        };
        backgroundJobServiceMock ??= new Mock<IBackgroundJobService>();
        securityAuditServiceMock ??= new Mock<ISecurityAuditService>();
        qrCodeServiceMock ??= new Mock<IQrCodeService>();
        
        if (geoLocationServiceMock == null)
        {
            geoLocationServiceMock = new Mock<IGeoLocationService>();

            geoLocationServiceMock
                .Setup(x => x.GetLocationAsync(It.IsAny<string?>()))
                .ReturnsAsync(new GeoLocationResult
                {
                    Country = "TestCountry",
                    City = "TestCity"
                });
        }

        return new AuthService(
            userManagerMock.Object,
            tokenServiceMock.Object,
            context,
            httpContextAccessor,
            backgroundJobServiceMock.Object,
            securityAuditServiceMock.Object,
            qrCodeServiceMock.Object,
            geoLocationServiceMock.Object,
            roleManagerMock.Object);
    }
}