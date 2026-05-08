using CarServiceBookingSystem.Application.DTOs.Services;
using CarServiceBookingSystem.Application.Validators.Services;
using FluentAssertions;

namespace CarServiceBookingSystem.Tests.Validators;

public class ServiceRequestValidatorTests
{
    [Fact]
    public void CreateService_Should_Pass_When_Request_Is_Valid()
    {
        var validator = new CreateServiceRequestValidator();

        var request = new CreateServiceRequest
        {
            Name = "Oil Change",
            Price = 120,
            DurationInMinutes = 30
        };

        var result = validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void CreateService_Should_Fail_When_Name_Is_Empty()
    {
        var validator = new CreateServiceRequestValidator();

        var request = new CreateServiceRequest
        {
            Name = "",
            Price = 120,
            DurationInMinutes = 30
        };

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(CreateServiceRequest.Name));
    }

    [Fact]
    public void CreateService_Should_Fail_When_Price_Is_Zero()
    {
        var validator = new CreateServiceRequestValidator();

        var request = new CreateServiceRequest
        {
            Name = "Oil Change",
            Price = 0,
            DurationInMinutes = 30
        };

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(CreateServiceRequest.Price));
    }

    [Fact]
    public void CreateService_Should_Fail_When_Duration_Is_Invalid()
    {
        var validator = new CreateServiceRequestValidator();

        var request = new CreateServiceRequest
        {
            Name = "Oil Change",
            Price = 120,
            DurationInMinutes = 0
        };

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(CreateServiceRequest.DurationInMinutes));
    }

    [Fact]
    public void UpdateService_Should_Pass_When_Request_Is_Valid()
    {
        var validator = new UpdateServiceRequestValidator();

        var request = new UpdateServiceRequest
        {
            Name = "Premium Oil Change",
            Price = 150,
            DurationInMinutes = 45
        };

        var result = validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }
}