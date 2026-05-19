using CarServiceBookingSystem.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace CarServiceBookingSystem.UnitTests.Services;

public class GeoLocationServiceTests
{
    [Fact]
    public async Task GetLocationAsync_Should_Return_Empty_When_Ip_Is_Null()
    {
        var service = CreateService();

        var result = await service.GetLocationAsync(null);

        result.Country.Should().BeNull();
        result.City.Should().BeNull();
    }

    [Theory]
    [InlineData("127.0.0.1")]
    [InlineData("::1")]
    public async Task GetLocationAsync_Should_Return_Localhost_For_Local_Ip(string ip)
    {
        var service = CreateService();

        var result = await service.GetLocationAsync(ip);

        result.Country.Should().Be("Localhost");
        result.City.Should().Be("Localhost");
    }

    [Theory]
    [InlineData("10.0.0.1")]
    [InlineData("192.168.1.10")]
    [InlineData("172.16.0.1")]
    [InlineData("172.31.255.255")]
    public async Task GetLocationAsync_Should_Return_PrivateNetwork_For_Private_Ip(string ip)
    {
        var service = CreateService();

        var result = await service.GetLocationAsync(ip);

        result.Country.Should().Be("Private Network");
        result.City.Should().Be("Private Network");
    }

    [Fact]
    public async Task GetLocationAsync_Should_Return_Empty_When_Database_File_Is_Missing()
    {
        var service = CreateService("missing-file.mmdb");

        var result = await service.GetLocationAsync("8.8.8.8");

        result.Country.Should().BeNull();
        result.City.Should().BeNull();
    }

    private static GeoLocationService CreateService(string? databasePath = null)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["GeoIp:DatabasePath"] = databasePath ?? ""
            })
            .Build();

        return new GeoLocationService(config);
    }
}