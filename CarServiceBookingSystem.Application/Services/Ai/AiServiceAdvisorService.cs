using CarServiceBookingSystem.Application.DTOs.Ai;
using CarServiceBookingSystem.Application.Interfaces.IAi;
using CarServiceBookingSystem.Application.Options;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace CarServiceBookingSystem.Application.Services.Ai;

public sealed class AiServiceAdvisorService : IAiServiceAdvisorService
{
    private readonly IAiServiceCatalogQuery _serviceCatalogQuery;
    private readonly IAiChatProvider _aiChatProvider;
    private readonly AiAdvisorOptions _options;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AiServiceAdvisorService(
        IAiServiceCatalogQuery serviceCatalogQuery,
        IAiChatProvider aiChatProvider,
        IOptions<AiAdvisorOptions> options)
    {
        _serviceCatalogQuery = serviceCatalogQuery;
        _aiChatProvider = aiChatProvider;
        _options = options.Value;
    }

    public async Task<ServiceAdvisorResponse> ChatAsync(
        ServiceAdvisorChatRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            return new ServiceAdvisorResponse
            {
                CanRecommend = false,
                Reply = "AI service advisor is currently disabled."
            };
        }

        var services = await _serviceCatalogQuery.GetActiveServicesAsync(
            cancellationToken);

        if (services.Count == 0)
        {
            return new ServiceAdvisorResponse
            {
                CanRecommend = false,
                Reply = "No active services are available right now."
            };
        }

        var serviceCatalogJson = JsonSerializer.Serialize(
            services,
            JsonOptions);

        var systemPrompt = BuildSystemPrompt();

        var userPrompt = BuildUserPrompt(
            request.Message,
            serviceCatalogJson);

        var rawAiJson = await _aiChatProvider.GetJsonChatCompletionAsync(
            systemPrompt,
            userPrompt,
            cancellationToken);

        var modelResult = TryParseModelResult(rawAiJson);

        if (modelResult is null)
        {
            return new ServiceAdvisorResponse
            {
                CanRecommend = false,
                Reply = "I could not safely match your symptoms to a service. Please describe the issue with more details.",
                FollowUpQuestions =
                [
                    "When does the problem happen?",
                    "Do you hear any noise?",
                    "Do you see any warning lights on the dashboard?"
                ]
            };
        }

        var serviceMap = services.ToDictionary(x => x.Id);

        var suggestions = modelResult.SuggestedServices
            .Where(x => serviceMap.ContainsKey(x.ServiceId))
            .OrderByDescending(x => x.Confidence)
            .Take(_options.MaxSuggestions)
            .Select(x =>
            {
                var service = serviceMap[x.ServiceId];

                return new AiSuggestedServiceDto
                {
                    ServiceId = service.Id,
                    ServiceName = service.Name,
                    Price = service.Price,
                    DurationInMinutes = service.DurationInMinutes,
                    Reason = x.Reason,
                    Confidence = Math.Clamp(x.Confidence, 0, 1),
                    BookingUrl = string.Format(
                        _options.BookingPathTemplate,
                        service.Id)
                };
            })
            .ToList();

        return new ServiceAdvisorResponse
        {
            CanRecommend = suggestions.Count > 0,
            Reply = string.IsNullOrWhiteSpace(modelResult.Reply)
                ? "Based on your description, these services may help."
                : modelResult.Reply,
            Urgency = NormalizeUrgency(modelResult.Urgency),
            SuggestedServices = suggestions,
            FollowUpQuestions = modelResult.FollowUpQuestions
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Take(5)
                .ToList(),
            Disclaimer = "This is not a final diagnosis. A technician should inspect the vehicle."
        };
    }

    private static string BuildSystemPrompt()
    {
        return """
        You are an AI service advisor for a car service booking platform.

        Rules:
        - Understand the user's car symptoms.
        - Recommend only services from the provided service catalog.
        - Never invent service IDs.
        - Never invent service names.
        - Never say the issue is confirmed.
        - If the issue may affect braking, steering, overheating, smoke, fuel smell, battery fire, or engine failure, set urgency to High.
        - If there is not enough information, ask follow-up questions.
        - Return valid JSON only.
        - Do not wrap JSON in markdown.
        - Do not include explanations outside JSON.

        Required JSON shape:
        {
          "reply": "short helpful message",
          "urgency": "Low | Medium | High | Unknown",
          "suggestedServices": [
            {
              "serviceId": 1,
              "reason": "why this service matches",
              "confidence": 0.85
            }
          ],
          "followUpQuestions": [
            "question 1"
          ]
        }
        """;
    }

    private static string BuildUserPrompt(
        string message,
        string serviceCatalogJson)
    {
        return $$"""
        User car problem:
        {{message}}

        Available service catalog JSON:
        {{serviceCatalogJson}}

        Return JSON only.
        """;
    }

    private static AdvisorModelResult? TryParseModelResult(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            return JsonSerializer.Deserialize<AdvisorModelResult>(
                json,
                JsonOptions);
        }
        catch
        {
            return null;
        }
    }

    private static string NormalizeUrgency(string? urgency)
    {
        if (string.IsNullOrWhiteSpace(urgency))
            return "Unknown";

        return urgency.Trim().ToLowerInvariant() switch
        {
            "low" => "Low",
            "medium" => "Medium",
            "high" => "High",
            _ => "Unknown"
        };
    }

    private sealed class AdvisorModelResult
    {
        public string Reply { get; init; } = string.Empty;

        public string Urgency { get; init; } = "Unknown";

        public List<AdvisorServiceSuggestion> SuggestedServices { get; init; } = [];

        public List<string> FollowUpQuestions { get; init; } = [];
    }

    private sealed class AdvisorServiceSuggestion
    {
        public int ServiceId { get; init; }

        public string Reason { get; init; } = string.Empty;

        public double Confidence { get; init; }
    }
}