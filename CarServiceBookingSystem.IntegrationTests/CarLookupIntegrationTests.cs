using FluentAssertions;
using System.Net;

namespace CarServiceBookingSystem.IntegrationTests;

public class CarLookupIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CarLookupIntegrationTests(CustomWebApplicationFactory factory)
    {
        factory.SeedCarLookupsAsync().GetAwaiter().GetResult();
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetBrands_Should_Return_Data()
    {
        var response = await _client.GetAsync("/api/v1/car-lookups/brands");

        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK, because: body);
        body.Should().Contain("Toyota");
    }

    [Fact]
    public async Task GetModels_Should_Return_Success()
    {
        var response = await _client.GetAsync("/api/v1/car-lookups/models/1");

        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK, because: body);
    }

    [Fact]
    public async Task GetYears_Should_Return_Success()
    {
        var response = await _client.GetAsync("/api/v1/car-lookups/years/1");

        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK, because: body);
    }

    [Fact]
    public async Task GetTrims_Should_Return_Success()
    {
        var response = await _client.GetAsync("/api/v1/car-lookups/trims/1");

        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK, because: body);
    }
}