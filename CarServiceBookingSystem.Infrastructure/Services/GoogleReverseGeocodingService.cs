using System.Net.Http.Json;
using System.Text.Json.Serialization;
using CarServiceBookingSystem.Application.DTOs.Location;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CarServiceBookingSystem.Infrastructure.Services;

public class GoogleReverseGeocodingService : IReverseGeocodingService
{
    private readonly HttpClient _httpClient;
    private readonly GoogleMapsOptions _options;
    private readonly ILogger<GoogleReverseGeocodingService> _logger;

    public GoogleReverseGeocodingService(
        HttpClient httpClient,
        IOptions<GoogleMapsOptions> options,
        ILogger<GoogleReverseGeocodingService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<ReverseGeocodeResponse?> ReverseGeocodeAsync(
        decimal latitude,
        decimal longitude,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            _logger.LogWarning("Google Maps API key is not configured.");
            return null;
        }

        var url =
            $"json?latlng={latitude},{longitude}&key={Uri.EscapeDataString(_options.ApiKey)}";

        GoogleGeocodeResponse? response;

        try
        {
            response = await _httpClient.GetFromJsonAsync<GoogleGeocodeResponse>(
                url,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Google reverse geocoding request failed.");
            return null;
        }

        if (response is null ||
            !string.Equals(response.Status, "OK", StringComparison.OrdinalIgnoreCase) ||
            response.Results.Count == 0)
        {
            _logger.LogWarning(
                "Google reverse geocoding returned status {Status}.",
                response?.Status);

            return null;
        }

        var bestResult = response.Results.First();

        var country = FindComponent(bestResult, "country");
        var city =
            FindComponent(bestResult, "locality") ??
            FindComponent(bestResult, "administrative_area_level_2") ??
            FindComponent(bestResult, "administrative_area_level_1");

        if (country is null || string.IsNullOrWhiteSpace(country.ShortName))
        {
            return null;
        }

        return new ReverseGeocodeResponse
        {
            CountryCode = country.ShortName.Trim().ToUpper(),
            CountryName = country.LongName?.Trim() ?? string.Empty,
            City = city?.LongName?.Trim(),
            FormattedAddress = bestResult.FormattedAddress
        };
    }

    private static GoogleAddressComponent? FindComponent(
        GoogleGeocodeResult result,
        string type)
    {
        return result.AddressComponents
            .FirstOrDefault(x => x.Types.Any(t =>
                string.Equals(t, type, StringComparison.OrdinalIgnoreCase)));
    }

    private class GoogleGeocodeResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("results")]
        public List<GoogleGeocodeResult> Results { get; set; } = [];
    }

    private class GoogleGeocodeResult
    {
        [JsonPropertyName("formatted_address")]
        public string? FormattedAddress { get; set; }

        [JsonPropertyName("address_components")]
        public List<GoogleAddressComponent> AddressComponents { get; set; } = [];
    }

    private class GoogleAddressComponent
    {
        [JsonPropertyName("long_name")]
        public string? LongName { get; set; }

        [JsonPropertyName("short_name")]
        public string? ShortName { get; set; }

        [JsonPropertyName("types")]
        public List<string> Types { get; set; } = [];
    }
}