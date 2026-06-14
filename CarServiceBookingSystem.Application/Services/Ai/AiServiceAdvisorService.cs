using CarServiceBookingSystem.Application.DTOs.Ai;
using CarServiceBookingSystem.Application.Interfaces.IAi;
using CarServiceBookingSystem.Application.Interfaces.IContext;
using CarServiceBookingSystem.Application.Options;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace CarServiceBookingSystem.Application.Services.Ai;

public sealed class AiServiceAdvisorService : IAiServiceAdvisorService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IAiServiceCatalogQuery _serviceCatalogQuery;
    private readonly IAiConversationRepository _conversationRepository;
    private readonly IAiChatProvider _aiChatProvider;
    private readonly AiAdvisorOptions _options;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AiServiceAdvisorService(
        ICurrentUserService currentUserService,
        IAiServiceCatalogQuery serviceCatalogQuery,
        IAiConversationRepository conversationRepository,
        IAiChatProvider aiChatProvider,
        IOptions<AiAdvisorOptions> options)
    {
        _currentUserService = currentUserService;
        _serviceCatalogQuery = serviceCatalogQuery;
        _conversationRepository = conversationRepository;
        _aiChatProvider = aiChatProvider;
        _options = options.Value;
    }

    public async Task<ServiceAdvisorResponse> ChatAsync(
        ServiceAdvisorChatRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return new ServiceAdvisorResponse
            {
                CanRecommend = false,
                Reply = "User is not authenticated."
            };
        }

        var conversation = await _conversationRepository.GetOrCreateConversationAsync(
            userId,
            request.ConversationId,
            request.CarId,
            request.Message,
            cancellationToken);

        await _conversationRepository.AddMessageAsync(
            conversation.Id,
            "User",
            request.Message,
            cancellationToken);

        if (!_options.Enabled)
        {
            var disabledAssistantMessage = await _conversationRepository.AddMessageAsync(
                conversation.Id,
                "Assistant",
                "AI service advisor is currently disabled.",
                cancellationToken);

            return new ServiceAdvisorResponse
            {
                ConversationId = conversation.Id,
                AssistantMessageId = disabledAssistantMessage.Id,
                CanRecommend = false,
                Reply = "AI service advisor is currently disabled."
            };
        }

        var services = await _serviceCatalogQuery.GetActiveServicesAsync(
            cancellationToken);

        if (services.Count == 0)
        {
            var noServicesAssistantMessage = await _conversationRepository.AddMessageAsync(
                conversation.Id,
                "Assistant",
                "No active services are available right now.",
                cancellationToken);

            return new ServiceAdvisorResponse
            {
                ConversationId = conversation.Id,
                AssistantMessageId = noServicesAssistantMessage.Id,
                CanRecommend = false,
                Reply = "No active services are available right now."
            };
        }

        var recentMessages = await _conversationRepository.GetRecentMessagesAsync(
            conversation.Id,
            take: 10,
            cancellationToken);

        var serviceCatalogJson = JsonSerializer.Serialize(
            services,
            JsonOptions);

        var recentMessagesJson = JsonSerializer.Serialize(
            recentMessages.Select(x => new
            {
                x.Role,
                x.Content
            }),
            JsonOptions);

        var systemPrompt = BuildSystemPrompt();

        var userPrompt = BuildUserPrompt(
            request.Message,
            recentMessagesJson,
            serviceCatalogJson);

        var rawAiJson = await _aiChatProvider.GetJsonChatCompletionAsync(
            systemPrompt,
            userPrompt,
            cancellationToken);

        var modelResult = TryParseModelResult(rawAiJson);

        if (modelResult is null)
        {
            const string fallbackReply =
                "I could not safely match your symptoms to a service. Please describe the issue with more details.";

            var fallbackAssistantMessage = await _conversationRepository.AddMessageAsync(
                conversation.Id,
                "Assistant",
                fallbackReply,
                cancellationToken);

            return new ServiceAdvisorResponse
            {
                ConversationId = conversation.Id,
                AssistantMessageId = fallbackAssistantMessage.Id,
                CanRecommend = false,
                Reply = fallbackReply,
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

        var reply = string.IsNullOrWhiteSpace(modelResult.Reply)
            ? "Based on your description, these services may help."
            : modelResult.Reply;

        var assistantMessage = await _conversationRepository.AddMessageAsync(
            conversation.Id,
            "Assistant",
            reply,
            cancellationToken);

        var recommendations = suggestions
            .Select(x => new CreateAiRecommendationDto
            {
                ServiceId = x.ServiceId,
                ServiceNameSnapshot = x.ServiceName,
                Reason = x.Reason,
                Confidence = x.Confidence,
                BookingUrl = x.BookingUrl
            })
            .ToList();

        await _conversationRepository.AddRecommendationsAsync(
            conversation.Id,
            assistantMessage.Id,
            recommendations,
            cancellationToken);

        return new ServiceAdvisorResponse
        {
            ConversationId = conversation.Id,
            AssistantMessageId = assistantMessage.Id,
            CanRecommend = suggestions.Count > 0,
            Reply = reply,
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
        - Use recent conversation history only as context.
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
        string recentMessagesJson,
        string serviceCatalogJson)
    {
        return $$"""
        Current user message:
        {{message}}

        Recent conversation messages JSON:
        {{recentMessagesJson}}

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