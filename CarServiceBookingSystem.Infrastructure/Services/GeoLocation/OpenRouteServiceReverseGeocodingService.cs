using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using CarServiceBookingSystem.Application.DTOs.Location;
using CarServiceBookingSystem.Application.Interfaces.IGeoLocation;
using CarServiceBookingSystem.Application.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CarServiceBookingSystem.Infrastructure.Services.GeoLocation;

public class OpenRouteServiceReverseGeocodingService : IReverseGeocodingService
{
    private readonly HttpClient _httpClient;
    private readonly OpenRouteServiceOptions _options;
    private readonly ILogger<OpenRouteServiceReverseGeocodingService> _logger;

    public OpenRouteServiceReverseGeocodingService(
        HttpClient httpClient,
        IOptions<OpenRouteServiceOptions> options,
        ILogger<OpenRouteServiceReverseGeocodingService> logger)
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
            _logger.LogWarning("OpenRouteService API key is not configured.");
            return null;
        }

        var lat = latitude.ToString(CultureInfo.InvariantCulture);
        var lon = longitude.ToString(CultureInfo.InvariantCulture);

        var url =
            $"geocode/reverse?point.lat={lat}" +
            $"&point.lon={lon}" +
            "&size=1";

        OpenRouteServiceGeocodeResponse? response;

        try
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);

            if (!httpRequest.Headers.TryAddWithoutValidation("Authorization", _options.ApiKey))
            {
                throw new InvalidOperationException("Could not add OpenRouteService authorization header.");
            }

            var httpResponse = await _httpClient.SendAsync(httpRequest, cancellationToken);

            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorBody = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

                _logger.LogWarning(
                    "OpenRouteService reverse geocoding returned {StatusCode}: {Body}",
                    httpResponse.StatusCode,
                    errorBody);

                return null;
            }

            response = await httpResponse.Content
                .ReadFromJsonAsync<OpenRouteServiceGeocodeResponse>(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OpenRouteService reverse geocoding request failed.");
            return null;
        }

        var feature = response?.Features.FirstOrDefault();

        if (feature?.Properties is null)
        {
            _logger.LogWarning(
                "OpenRouteService reverse geocoding returned no result for {Latitude}, {Longitude}.",
                latitude,
                longitude);

            return null;
        }

        var properties = feature.Properties;

        var countryCode = properties.CountryCode;

        if (string.IsNullOrWhiteSpace(countryCode))
        {
            _logger.LogWarning(
                "OpenRouteService reverse geocoding did not return country code for {Latitude}, {Longitude}.",
                latitude,
                longitude);

            return null;
        }

        var city =
            properties.Locality ??
            properties.LocalAdmin ??
            properties.County ??
            properties.Region;

        return new ReverseGeocodeResponse
        {
            CountryCode = NormalizeCountryCode(countryCode),
            CountryName = properties.Country?.Trim() ?? string.Empty,
            City = city?.Trim(),
            FormattedAddress = properties.Label
        };
    }

    private class OpenRouteServiceGeocodeResponse
    {
        [JsonPropertyName("features")]
        public List<OpenRouteServiceGeocodeFeature> Features { get; set; } = [];
    }

    private class OpenRouteServiceGeocodeFeature
    {
        [JsonPropertyName("properties")]
        public OpenRouteServiceGeocodeProperties Properties { get; set; } = new();
    }

    private class OpenRouteServiceGeocodeProperties
    {
        [JsonPropertyName("label")]
        public string? Label { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("country_a")]
        public string? CountryCode { get; set; }

        [JsonPropertyName("region")]
        public string? Region { get; set; }

        [JsonPropertyName("county")]
        public string? County { get; set; }

        [JsonPropertyName("locality")]
        public string? Locality { get; set; }

        [JsonPropertyName("localadmin")]
        public string? LocalAdmin { get; set; }
    }

    private static string NormalizeCountryCode(string countryCode)
    {
        var value = countryCode.Trim().ToUpperInvariant();

        if (value.Length == 2)
        {
            return value;
        }

        if (value.Length == 3)
        {
            var region = CultureInfo
                .GetCultures(CultureTypes.SpecificCultures)
                .Select(culture => new RegionInfo(culture.Name))
                .FirstOrDefault(region =>
                    string.Equals(
                        region.ThreeLetterISORegionName,
                        value,
                        StringComparison.OrdinalIgnoreCase));

            if (region is not null)
            {
                return region.TwoLetterISORegionName;
            }
        }

        return value;
    }
}