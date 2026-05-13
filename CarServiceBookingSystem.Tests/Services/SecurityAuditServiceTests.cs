using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Infrastructure.Services;
using CarServiceBookingSystem.UnitTests.TestHelpers;
using FluentAssertions;

namespace CarServiceBookingSystem.UnitTests.Services;

public class SecurityAuditServiceTests
{
    [Fact]
    public async Task LogAsync_Should_Create_Security_Audit_Log()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var service = new SecurityAuditService(context);

        await service.LogAsync(
            userId: "user-id",
            eventType: "LoginSuccess",
            ipAddress: "127.0.0.1",
            device: "Chrome",
            details: "Test login");

        context.SecurityAuditLogs.Should().HaveCount(1);

        var log = context.SecurityAuditLogs.First();

        log.UserId.Should().Be("user-id");
        log.EventType.Should().Be("LoginSuccess");
        log.IpAddress.Should().Be("127.0.0.1");
        log.Device.Should().Be("Chrome");
        log.Details.Should().Be("Test login");
    }

    [Fact]
    public async Task GetLogsAsync_Should_Return_Paged_Logs()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        await context.SecurityAuditLogs.AddRangeAsync(
    new SecurityAuditLog
    {
        UserId = "user-1",
        EventType = "LoginSuccess",
        IpAddress = "127.0.0.1",
        Device = "Chrome"
    },
    new SecurityAuditLog
    {
        UserId = "user-2",
        EventType = "LoginFailed",
        IpAddress = "127.0.0.2",
        Device = "Firefox"
    });

        await context.SaveChangesAsync();

        var service = new SecurityAuditQueryService(context);

        var result = await service.GetLogsAsync(new PagedRequest
        {
            PageNumber = 1,
            PageSize = 10
        });

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(2);
        result.Data.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task GetLogsAsync_Should_Filter_By_Search()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        await context.SecurityAuditLogs.AddRangeAsync(
     new SecurityAuditLog
     {
         UserId = "user-1",
         EventType = "LoginSuccess",
         IpAddress = "127.0.0.1",
         Device = "Chrome"
     },
     new SecurityAuditLog
     {
         UserId = "user-2",
         EventType = "LoginFailed",
         IpAddress = "127.0.0.2",
         Device = "Firefox"
     });

        await context.SaveChangesAsync();

        var service = new SecurityAuditQueryService(context);

        var result = await service.GetLogsAsync(new PagedRequest
        {
            PageNumber = 1,
            PageSize = 10,
            Search = "PasswordChanged"
        });

        result.Success.Should().BeTrue();
        result.Data!.Items.Should().HaveCount(1);
        result.Data.Items[0].EventType.Should().Be("PasswordChanged");
    }
}