using CarServiceBookingSystem.Application.DTOs.Bookings;
using CarServiceBookingSystem.Application.Validators.Bookings;
using CarServiceBookingSystem.Domain.Enums;
using FluentAssertions;

namespace CarServiceBookingSystem.Tests.Validators;

public class CreateBookingRequestValidatorTests
{
    private readonly CreateBookingRequestValidator _validator = new();

    [Fact]
    public void Should_Pass_When_Request_Is_Valid()
    {
        var request = new CreateBookingRequest
        {
            CarId = 1,
            ServiceId = 1,
            LocationType = ServiceLocationType.AtWorkshop,
            StartDate = DateTime.UtcNow.AddDays(1)
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Fail_When_CarId_Is_Invalid()
    {
        var request = new CreateBookingRequest
        {
            CarId = 0,
            ServiceId = 1,
            LocationType = ServiceLocationType.AtWorkshop,
            StartDate = DateTime.UtcNow.AddDays(1)
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(CreateBookingRequest.CarId));
    }

    [Fact]
    public void Should_Fail_When_ServiceId_Is_Invalid()
    {
        var request = new CreateBookingRequest
        {
            CarId = 1,
            ServiceId = 0,
            LocationType = ServiceLocationType.AtWorkshop,
            StartDate = DateTime.UtcNow.AddDays(1)
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(CreateBookingRequest.ServiceId));
    }

    [Fact]
    public void Should_Fail_When_StartDate_Is_In_Past()
    {
        var request = new CreateBookingRequest
        {
            CarId = 1,
            ServiceId = 1,
            LocationType = ServiceLocationType.AtWorkshop,
            StartDate = DateTime.UtcNow.AddDays(-1)
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(CreateBookingRequest.StartDate));
    }
}