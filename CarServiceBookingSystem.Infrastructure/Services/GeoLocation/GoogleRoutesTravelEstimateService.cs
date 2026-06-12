using System.Net.Http.Json;
using System.Text.Json.Serialization;
using CarServiceBookingSystem.Application.DTOs.Travel;
using CarServiceBookingSystem.Application.Interfaces.IGeoLocation;
using CarServiceBookingSystem.Application.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CarServiceBookingSystem.Infrastructure.Services.GeoLocation;

public class GoogleRoutesTravelEstimateService : ITravelEstimateService
{
    private readonly HttpClient _httpClient;
    private readonly GoogleMapsOptions _options;
    private readonly ILogger<GoogleRoutesTravelEstimateService> _logger;

    public GoogleRoutesTravelEstimateService(
        HttpClient httpClient,
        IOptions<GoogleMapsOptions> options,
        ILogger<GoogleRoutesTravelEstimateService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<TravelEstimateResponse> EstimateAsync(
        decimal originLatitude,
        decimal originLongitude,
        decimal destinationLatitude,
        decimal destinationLongitude,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw new InvalidOperationException("Google Maps API key is not configured.");
        }

        var request = new GoogleRoutesRequest
        {
            Origin = new GoogleWaypoint
            {
                Location = new GoogleLocation
                {
                    LatLng = new GoogleLatLng
                    {
                        Latitude = (double)originLatitude,
                        Longitude = (double)originLongitude
                    }
                }
            },
            Destination = new GoogleWaypoint
            {
                Location = new GoogleLocation
                {
                    LatLng = new GoogleLatLng
                    {
                        Latitude = (double)destinationLatitude,
                        Longitude = (double)destinationLongitude
                    }
                }
            },
            TravelMode = "DRIVE",
            RoutingPreference = "TRAFFIC_AWARE"
        };

        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            "directions/v2:computeRoutes");

        httpRequest.Headers.Add("X-Goog-Api-Key", _options.ApiKey);
        httpRequest.Headers.Add(
            "X-Goog-FieldMask",
            "routes.duration,routes.distanceMeters");

        httpRequest.Content = JsonContent.Create(request);

        HttpResponseMessage httpResponse;

        try
        {
            httpResponse = await _httpClient.SendAsync(
                httpRequest,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Google Routes API request failed.");
            throw;
        }

        if (!httpResponse.IsSuccessStatusCode)
        {
            var errorBody = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            _logger.LogWarning(
                "Google Routes API returned {StatusCode}: {Body}",
                httpResponse.StatusCode,
                errorBody);

            throw new InvalidOperationException("Google Routes API request failed.");
        }

        var routesResponse = await httpResponse.Content
            .ReadFromJsonAsync<GoogleRoutesResponse>(cancellationToken);

        var route = routesResponse?.Routes.FirstOrDefault();

        if (route is null)
        {
            throw new InvalidOperationException("Google Routes API did not return a route.");
        }

        var distanceKm = Math.Round(route.DistanceMeters / 1000d, 2);
        var durationMinutes = ParseDurationToMinutes(route.Duration);

        return new TravelEstimateResponse
        {
            DistanceKm = distanceKm,
            EstimatedTravelTimeMinutes = durationMinutes
        };
    }

    private static int ParseDurationToMinutes(string? duration)
    {
        if (string.IsNullOrWhiteSpace(duration))
        {
            return 0;
        }

        // Google returns duration like "123s".
        if (duration.EndsWith("s", StringComparison.OrdinalIgnoreCase) &&
            double.TryParse(duration[..^1], out var seconds))
        {
            return Math.Max(1, (int)Math.Ceiling(seconds / 60d));
        }

        return 0;
    }

    private class GoogleRoutesRequest
    {
        [JsonPropertyName("origin")]
        public GoogleWaypoint Origin { get; set; } = null!;

        [JsonPropertyName("destination")]
        public GoogleWaypoint Destination { get; set; } = null!;

        [JsonPropertyName("travelMode")]
        public string TravelMode { get; set; } = "DRIVE";

        [JsonPropertyName("routingPreference")]
        public string RoutingPreference { get; set; } = "TRAFFIC_AWARE";
    }

    private class GoogleWaypoint
    {
        [JsonPropertyName("location")]
        public GoogleLocation Location { get; set; } = null!;
    }

    private class GoogleLocation
    {
        [JsonPropertyName("latLng")]
        public GoogleLatLng LatLng { get; set; } = null!;
    }

    private class GoogleLatLng
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
    }

    private class GoogleRoutesResponse
    {
        [JsonPropertyName("routes")]
        public List<GoogleRoute> Routes { get; set; } = [];
    }

    private class GoogleRoute
    {
        [JsonPropertyName("duration")]
        public string? Duration { get; set; }

        [JsonPropertyName("distanceMeters")]
        public int DistanceMeters { get; set; }
    }
}