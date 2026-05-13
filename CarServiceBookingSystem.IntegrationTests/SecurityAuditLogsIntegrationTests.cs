using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;

namespace CarServiceBookingSystem.IntegrationTests;

public class SecurityAuditLogsIntegrationTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public SecurityAuditLogsIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetSecurityAuditLogs_Should_Return_Success_When_User_Is_Admin()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                TestAuthHelper.GenerateJwt(
                    "admin-user-id",
                    "admin@test.com",
                    "Admin"));

        using var scope = _factory.Services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        context.SecurityAuditLogs.Add(new SecurityAuditLog
        {
            UserId = "user-id",
            EventType = "LoginSuccess",
            IpAddress = "127.0.0.1",
            Device = "Chrome",
            Details = "Test audit log"
        });

        await context.SaveChangesAsync();

        var response = await _client.GetAsync(
            "/api/v1/security-audit-logs?pageNumber=1&pageSize=10");

        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK, because: body);
        body.Should().Contain("LoginSuccess");
    }

    [Fact]
    public async Task GetSecurityAuditLogs_Should_Return_Forbidden_When_User_Is_Not_Admin()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                TestAuthHelper.GenerateJwt(
                    "normal-user-id",
                    "user@test.com",
                    "User"));

        var response = await _client.GetAsync(
            "/api/v1/security-audit-logs?pageNumber=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}