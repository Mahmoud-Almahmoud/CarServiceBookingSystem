using System.Net.Http.Json;
using System.Text.Json.Serialization;
using CarServiceBookingSystem.Application.DTOs.Travel;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CarServiceBookingSystem.Infrastructure.Services;

public class OpenRouteServiceTravelEstimateService : ITravelEstimateService
{
    private readonly HttpClient _httpClient;
    private readonly OpenRouteServiceOptions _options;
    private readonly ILogger<OpenRouteServiceTravelEstimateService> _logger;

    public OpenRouteServiceTravelEstimateService(
        HttpClient httpClient,
        IOptions<OpenRouteServiceOptions> options,
        ILogger<OpenRouteServiceTravelEstimateService> logger)
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
            throw new InvalidOperationException("OpenRouteService API key is not configured.");
        }

        var request = new OpenRouteServiceDirectionsRequest
        {
            Coordinates =
            [
                [
                    (double)originLongitude,
                    (double)originLatitude
                ],
                [
                    (double)destinationLongitude,
                    (double)destinationLatitude
                ]
            ],
            Instructions = false
        };

        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            "v2/directions/driving-car/json");

        if (!httpRequest.Headers.TryAddWithoutValidation("Authorization", _options.ApiKey))
        {
            throw new InvalidOperationException("Could not add OpenRouteService authorization header.");
        }
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
            _logger.LogError(ex, "OpenRouteService directions request failed.");
            throw;
        }

        if (!httpResponse.IsSuccessStatusCode)
        {
            var errorBody = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            _logger.LogWarning(
                "OpenRouteService directions API returned {StatusCode}: {Body}",
                httpResponse.StatusCode,
                errorBody);

            throw new InvalidOperationException("OpenRouteService directions request failed.");
        }

        var response = await httpResponse.Content
            .ReadFromJsonAsync<OpenRouteServiceDirectionsResponse>(cancellationToken);

        var route = response?.Routes.FirstOrDefault();

        if (route?.Summary is null)
        {
            throw new InvalidOperationException("OpenRouteService did not return a route summary.");
        }

        return new TravelEstimateResponse
        {
            DistanceKm = Math.Round(route.Summary.Distance / 1000d, 2),
            EstimatedTravelTimeMinutes = Math.Max(
                1,
                (int)Math.Ceiling(route.Summary.Duration / 60d))
        };
    }

    private class OpenRouteServiceDirectionsRequest
    {
        [JsonPropertyName("coordinates")]
        public List<List<double>> Coordinates { get; set; } = [];

        [JsonPropertyName("instructions")]
        public bool Instructions { get; set; }
    }

    private class OpenRouteServiceDirectionsResponse
    {
        [JsonPropertyName("routes")]
        public List<OpenRouteServiceRoute> Routes { get; set; } = [];
    }

    private class OpenRouteServiceRoute
    {
        [JsonPropertyName("summary")]
        public OpenRouteServiceRouteSummary? Summary { get; set; }
    }

    private class OpenRouteServiceRouteSummary
    {
        [JsonPropertyName("distance")]
        public double Distance { get; set; }

        [JsonPropertyName("duration")]
        public double Duration { get; set; }
    }
}