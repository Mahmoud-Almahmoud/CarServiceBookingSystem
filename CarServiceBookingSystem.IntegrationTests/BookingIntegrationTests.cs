using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Domain.Enums;
using CarServiceBookingSystem.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CarServiceBookingSystem.IntegrationTests;

public class BookingIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public BookingIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateBooking_Should_Return_Success_When_User_Is_Authenticated()
    {
        const string userId = "test-user-id";
        const string email = "test@test.com";

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                TestAuthHelper.GenerateJwt(userId, email));

        using var scope = _factory.Services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        var service = new Service
        {
            Name = "Oil Change",
            Price = 120,
            DurationInMinutes = 30
        };

        var car = new Car
        {
            UserId = userId,
            CarTrimId = 1,
            PlateNumber = "12345 AD"
        };

        context.Services.Add(service);
        context.Cars.Add(car);
        await context.SaveChangesAsync();

        var request = new
        {
            carId = car.Id,
            serviceId = service.Id,
            locationType = 1,
            startDate = DateTime.UtcNow.AddDays(1)
        };

        var response = await _client.PostAsJsonAsync(
            "/api/v1/bookings",
            request);

        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK, because: body);
        body.Should().Contain("Booking created successfully");
    }

    [Fact]
    public async Task GetMyBookings_Should_Return_Only_Current_User_Bookings()
    {
        const string userId = "test-user-id";
        const string email = "test@test.com";

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                TestAuthHelper.GenerateJwt(userId, email));

        using var scope = _factory.Services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        var service = new Service
        {
            Name = "Oil Change",
            Price = 120,
            DurationInMinutes = 30
        };

        var myCar = new Car
        {
            UserId = userId,
            CarTrimId = 1,
            PlateNumber = "MY-123"
        };

        var otherCar = new Car
        {
            UserId = "other-user-id",
            CarTrimId = 1,
            PlateNumber = "OTHER-999"
        };

        context.Services.Add(service);
        context.Cars.AddRange(myCar, otherCar);
        await context.SaveChangesAsync();

        context.Bookings.AddRange(
            new Booking
            {
                UserId = userId,
                CarId = myCar.Id,
                ServiceId = service.Id,
                LocationType = ServiceLocationType.AtWorkshop,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(1).AddMinutes(30),
                Status = BookingStatus.Pending
            },
            new Booking
            {
                UserId = "other-user-id",
                CarId = otherCar.Id,
                ServiceId = service.Id,
                LocationType = ServiceLocationType.AtWorkshop,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(1).AddMinutes(30),
                Status = BookingStatus.Pending
            });

        await context.SaveChangesAsync();

        var response = await _client.GetAsync("/api/v1/bookings/my-bookings");

        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK, because: body);

        body.Should().Contain("MY-123");
        body.Should().NotContain("OTHER-999");
    }
    [Fact]
    public async Task GetAllBookings_Should_Return_Success_When_User_Is_Admin()
    {
        const string adminUserId = "admin-user-id";
        const string adminEmail = "admin@test.com";

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                TestAuthHelper.GenerateJwt(adminUserId, adminEmail, "Admin"));

        using var scope = _factory.Services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        var service = new Service
        {
            Name = "Oil Change",
            Price = 120,
            DurationInMinutes = 30
        };

        var car = new Car
        {
            UserId = "customer-user-id",
            CarTrimId = 1,
            PlateNumber = "ADMIN-TEST"
        };

        context.Services.Add(service);
        context.Cars.Add(car);
        await context.SaveChangesAsync();

        context.Bookings.Add(new Booking
        {
            UserId = "customer-user-id",
            CarId = car.Id,
            ServiceId = service.Id,
            LocationType = ServiceLocationType.AtWorkshop,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(1).AddMinutes(30),
            Status = BookingStatus.Pending
        });

        await context.SaveChangesAsync();

        var response = await _client.GetAsync("/api/v1/bookings");

        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK, because: body);

        body.Should().Contain("ADMIN-TEST");
    }
    [Fact]
    public async Task GetAllBookings_Should_Return_Forbidden_When_User_Is_Not_Admin()
    {
        const string userId = "normal-user-id";
        const string email = "user@test.com";

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                TestAuthHelper.GenerateJwt(userId, email, "User"));

        var response = await _client.GetAsync("/api/v1/bookings");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}