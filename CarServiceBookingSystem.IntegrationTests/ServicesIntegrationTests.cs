using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace CarServiceBookingSystem.IntegrationTests;

public class ServicesIntegrationTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ServicesIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetServices_Should_Return_Seeded_Data()
    {
        using var scope = _factory.Services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        context.Services.Add(new Service
        {
            Name = "Oil Change",
            Price = 120,
            DurationInMinutes = 30
        });

        await context.SaveChangesAsync();

        var response = await _client.GetAsync("/api/v1/services");

        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK, because: body);

        body.Should().Contain("Oil Change");
    }

    [Fact]
    public async Task GetServices_Should_Support_Search_Filter()
    {
        using var scope = _factory.Services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        context.Services.AddRange(
            new Service
            {
                Name = "Oil Change",
                Price = 120,
                DurationInMinutes = 30
            },
            new Service
            {
                Name = "Brake Inspection",
                Price = 150,
                DurationInMinutes = 60
            });

        await context.SaveChangesAsync();

        var response = await _client.GetAsync("/api/v1/services?search=Brake");

        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK, because: body);

        body.Should().Contain("Brake Inspection");
        body.Should().NotContain("Oil Change");
    }
    [Fact]
    public async Task GetServices_Should_Support_Pagination()
    {
        using var scope = _factory.Services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        context.Services.AddRange(
            new Service
            {
                Name = "Service 1",
                Price = 100,
                DurationInMinutes = 30
            },
            new Service
            {
                Name = "Service 2",
                Price = 200,
                DurationInMinutes = 45
            });

        await context.SaveChangesAsync();

        var response = await _client.GetAsync("/api/v1/services?pageNumber=1&pageSize=1");

        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK, because: body);

        body.Should().Contain("\"pageSize\":1");
    }
}