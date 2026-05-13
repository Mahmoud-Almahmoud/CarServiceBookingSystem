using CarServiceBookingSystem.Application.DTOs.Bookings;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Domain.Enums;
using CarServiceBookingSystem.Infrastructure.Identity;
using CarServiceBookingSystem.Infrastructure.Services;
using CarServiceBookingSystem.UnitTests.TestHelpers;
using FluentAssertions;
using Moq;

namespace CarServiceBookingSystem.UnitTests.Services;

public class BookingServiceTests
{
    [Fact]
    public async Task CreateAsync_Should_Create_Booking()
    {
        // Arrange
        await using var context = TestDbContextFactory.CreateDbContext();

        var currentUserMock = new Mock<ICurrentUserService>();

        currentUserMock.Setup(x => x.UserId)
            .Returns("test-user-id");

        var emailServiceMock = new Mock<IEmailService>();
        var backgroundJobMock = new Mock<IBackgroundJobService>();

        var bookingService = new BookingService(
            context,
            currentUserMock.Object,
            backgroundJobMock.Object,
            emailServiceMock.Object);

        var service = new Service
        {
            Name = "Oil Change",
            Price = 120,
            DurationInMinutes = 30
        };

        context.Services.Add(service);

        var car = new Car
        {
            UserId = "test-user-id",
            CarTrimId = 1,
            PlateNumber = "12345"
        };

        context.Cars.Add(car);

        await context.SaveChangesAsync();

        var request = new CreateBookingRequest
        {
            CarId = car.Id,
            ServiceId = service.Id,
            LocationType = ServiceLocationType.OnSite,
            StartDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var result = await bookingService.CreateAsync(request);

        // Assert
        result.Success.Should().BeTrue();

        result.Data.Should().NotBeNull();

        result.Data!.Status.Should().Be(BookingStatus.Pending);

        context.Bookings.Count().Should().Be(1);
    }

    [Fact]
    public async Task CreateAsync_Should_Fail_When_Car_Not_Found()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var currentUserMock = new Mock<ICurrentUserService>();
        currentUserMock.Setup(x => x.UserId).Returns("test-user-id");

        var emailServiceMock = new Mock<IEmailService>();
        var backgroundJobMock = new Mock<IBackgroundJobService>();

        var bookingService = new BookingService(
            context,
            currentUserMock.Object,
            backgroundJobMock.Object,
            emailServiceMock.Object);

        var request = new CreateBookingRequest
        {
            CarId = 999,
            ServiceId = 1,
            LocationType = ServiceLocationType.AtWorkshop,
            StartDate = DateTime.UtcNow.AddDays(1)
        };

        var result = await bookingService.CreateAsync(request);

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Car not found");
    }

    [Fact]
    public async Task CreateAsync_Should_Fail_When_Service_Not_Found()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var currentUserMock = new Mock<ICurrentUserService>();
        currentUserMock.Setup(x => x.UserId).Returns("test-user-id");

        var emailServiceMock = new Mock<IEmailService>();
        var backgroundJobMock = new Mock<IBackgroundJobService>();

        var bookingService = new BookingService(
            context,
            currentUserMock.Object,
            backgroundJobMock.Object,
            emailServiceMock.Object);

        var car = new Car
        {
            UserId = "test-user-id",
            CarTrimId = 1,
            PlateNumber = "12345"
        };

        context.Cars.Add(car);
        await context.SaveChangesAsync();

        var request = new CreateBookingRequest
        {
            CarId = car.Id,
            ServiceId = 999,
            LocationType = ServiceLocationType.AtWorkshop,
            StartDate = DateTime.UtcNow.AddDays(1)
        };

        var result = await bookingService.CreateAsync(request);

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Service not found");
    }
    [Fact]
    public async Task CreateAsync_Should_Fail_When_StartDate_Is_In_Past()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var currentUserMock = new Mock<ICurrentUserService>();
        currentUserMock.Setup(x => x.UserId).Returns("test-user-id");

        var emailServiceMock = new Mock<IEmailService>();
        var backgroundJobMock = new Mock<IBackgroundJobService>();

        var bookingService = new BookingService(
            context,
            currentUserMock.Object,
            backgroundJobMock.Object,
            emailServiceMock.Object);

        var service = new Service
        {
            Name = "Oil Change",
            Price = 120,
            DurationInMinutes = 30
        };

        var car = new Car
        {
            UserId = "test-user-id",
            CarTrimId = 1,
            PlateNumber = "12345"
        };

        context.Services.Add(service);
        context.Cars.Add(car);
        await context.SaveChangesAsync();

        var request = new CreateBookingRequest
        {
            CarId = car.Id,
            ServiceId = service.Id,
            LocationType = ServiceLocationType.AtWorkshop,
            StartDate = DateTime.UtcNow.AddDays(-1)
        };

        var result = await bookingService.CreateAsync(request);

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Start date must be in the future");
    }
    [Fact]
    public async Task UpdateStatusAsync_Should_Update_Booking_Status()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var currentUserMock = new Mock<ICurrentUserService>();
        currentUserMock.Setup(x => x.UserId).Returns("admin-user-id");

        var emailServiceMock = new Mock<IEmailService>();
        var backgroundJobMock = new Mock<IBackgroundJobService>();

        var bookingService = new BookingService(
            context,
            currentUserMock.Object,
            backgroundJobMock.Object,
            emailServiceMock.Object);

        var service = new Service
        {
            Name = "Oil Change",
            Price = 120,
            DurationInMinutes = 30
        };

        var car = new Car
        {
            UserId = "test-user-id",
            CarTrimId = 1,
            PlateNumber = "12345"
        };

        context.Services.Add(service);
        context.Cars.Add(car);
        await context.SaveChangesAsync();

        var booking = new Booking
        {
            UserId = "test-user-id",
            CarId = car.Id,
            ServiceId = service.Id,
            LocationType = ServiceLocationType.AtWorkshop,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(1).AddMinutes(30),
            Status = BookingStatus.Pending
        };

        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        var request = new UpdateBookingStatusRequest
        {
            Status = BookingStatus.Confirmed
        };

        var result = await bookingService.UpdateStatusAsync(booking.Id, request);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Status.Should().Be(BookingStatus.Confirmed);

        var updatedBooking = await context.Bookings.FindAsync(booking.Id);
        updatedBooking!.Status.Should().Be(BookingStatus.Confirmed);
    }
    [Fact]
    public async Task UpdateStatusAsync_Should_Queue_Email_When_Booking_Is_Confirmed()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var currentUserMock = new Mock<ICurrentUserService>();
        currentUserMock.Setup(x => x.UserId).Returns("admin-user-id");

        var emailServiceMock = new Mock<IEmailService>();
        var backgroundJobMock = new Mock<IBackgroundJobService>();

        var bookingService = new BookingService(
            context,
            currentUserMock.Object,
            backgroundJobMock.Object,
            emailServiceMock.Object);

        var user = new ApplicationUser
        {
            Id = "test-user-id",
            FullName = "Test User",
            UserName = "test@test.com",
            Email = "test@test.com"
        };

        context.Users.Add(user);

        var service = new Service
        {
            Name = "Oil Change",
            Price = 120,
            DurationInMinutes = 30
        };

        var car = new Car
        {
            UserId = "test-user-id",
            CarTrimId = 1,
            PlateNumber = "12345"
        };

        context.Services.Add(service);
        context.Cars.Add(car);
        await context.SaveChangesAsync();

        var booking = new Booking
        {
            UserId = "test-user-id",
            CarId = car.Id,
            ServiceId = service.Id,
            LocationType = ServiceLocationType.AtWorkshop,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(1).AddMinutes(30),
            Status = BookingStatus.Pending
        };

        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        var request = new UpdateBookingStatusRequest
        {
            Status = BookingStatus.Confirmed
        };

        await bookingService.UpdateStatusAsync(booking.Id, request);

        backgroundJobMock.Verify(x =>
            x.EnqueueEmail(
                "test@test.com",
                "Booking Confirmed",
                It.Is<string>(body => body.Contains($"Booking ID: {booking.Id}"))),
            Times.Once);
    }
}