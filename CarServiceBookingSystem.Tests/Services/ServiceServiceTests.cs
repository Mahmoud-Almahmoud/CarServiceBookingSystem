using CarServiceBookingSystem.Application.DTOs.Services;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Infrastructure.Services;
using CarServiceBookingSystem.UnitTests.TestHelpers;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;

namespace CarServiceBookingSystem.UnitTests.Services;

public class ServiceServiceTests
{
    [Fact]
    public async Task CreateAsync_Should_Create_Service()
    {
        // Arrange
        await using var context = TestDbContextFactory.CreateDbContext();

        var cache = new MemoryCache(new MemoryCacheOptions());

        var service = new ServiceService(context, cache);

        var request = new CreateServiceRequest
        {
            Name = "Oil Change",
            Price = 120,
            DurationInMinutes = 30
        };

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Oil Change");
        result.Data.Price.Should().Be(120);

        context.Services.Count().Should().Be(1);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Service()
    {
        // Arrange
        await using var context = TestDbContextFactory.CreateDbContext();

        var cache = new MemoryCache(new MemoryCacheOptions());

        var serviceService = new ServiceService(context, cache);

        var service = new Service
        {
            Name = "Oil Change",
            Price = 100,
            DurationInMinutes = 30
        };

        context.Services.Add(service);
        await context.SaveChangesAsync();

        var request = new UpdateServiceRequest
        {
            Name = "Premium Oil Change",
            Price = 150,
            DurationInMinutes = 45
        };

        // Act
        var result = await serviceService.UpdateAsync(service.Id, request);

        // Assert
        result.Success.Should().BeTrue();

        var updatedService = await context.Services.FindAsync(service.Id);

        updatedService.Should().NotBeNull();
        updatedService!.Name.Should().Be("Premium Oil Change");
        updatedService.Price.Should().Be(150);
        updatedService.DurationInMinutes.Should().Be(45);
    }

    [Fact]
    public async Task DeleteAsync_Should_Delete_Service()
    {
        // Arrange
        await using var context = TestDbContextFactory.CreateDbContext();

        var cache = new MemoryCache(new MemoryCacheOptions());

        var serviceService = new ServiceService(context, cache);

        var service = new Service
        {
            Name = "Brake Inspection",
            Price = 150,
            DurationInMinutes = 60
        };

        context.Services.Add(service);
        await context.SaveChangesAsync();

        // Act
        var result = await serviceService.DeleteAsync(service.Id);

        // Assert
        result.Success.Should().BeTrue();

        var deletedService = await context.Services.FindAsync(service.Id);

        deletedService.Should().NotBeNull();
        deletedService!.IsDeleted.Should().BeTrue();
    }
}