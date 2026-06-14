using CarServiceBookingSystem.Application.DTOs.Ai;
using CarServiceBookingSystem.Application.Interfaces.IAi;
using CarServiceBookingSystem.Domain.Enums;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CarServiceBookingSystem.Infrastructure.Services.Ai;

public sealed class AiAnalyticsQuery : IAiAnalyticsQuery
{
    private readonly ApplicationDbContext _context;

    public AiAnalyticsQuery(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AiAnalyticsOverviewDto> GetOverviewAsync(
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var last7Days = now.AddDays(-7);

        var totalConversations = await _context.AiConversations
            .AsNoTracking()
            .CountAsync(cancellationToken);

        var totalMessages = await _context.AiConversationMessages
            .AsNoTracking()
            .CountAsync(cancellationToken);

        var totalRecommendations = await _context.AiServiceRecommendations
            .AsNoTracking()
            .CountAsync(cancellationToken);

        var helpfulFeedbackCount = await _context.AiServiceRecommendations
            .AsNoTracking()
            .CountAsync(x => x.FeedbackValue == AiRecommendationFeedbackValue.Helpful, cancellationToken);

        var notHelpfulFeedbackCount = await _context.AiServiceRecommendations
            .AsNoTracking()
            .CountAsync(x => x.FeedbackValue == AiRecommendationFeedbackValue.NotHelpful, cancellationToken);

        var feedbackCount = helpfulFeedbackCount + notHelpfulFeedbackCount;

        var averageConfidence = await _context.AiServiceRecommendations
            .AsNoTracking()
            .Select(x => (double?)x.Confidence)
            .AverageAsync(cancellationToken) ?? 0;

        var conversationsLast7Days = await _context.AiConversations
            .AsNoTracking()
            .CountAsync(x => x.CreatedAt >= last7Days, cancellationToken);

        var recommendationsLast7Days = await _context.AiServiceRecommendations
            .AsNoTracking()
            .CountAsync(x => x.CreatedAt >= last7Days, cancellationToken);

        return new AiAnalyticsOverviewDto
        {
            TotalConversations = totalConversations,
            TotalMessages = totalMessages,
            TotalRecommendations = totalRecommendations,
            HelpfulFeedbackCount = helpfulFeedbackCount,
            NotHelpfulFeedbackCount = notHelpfulFeedbackCount,
            FeedbackRate = totalRecommendations == 0
                ? 0
                : Math.Round((double)feedbackCount / totalRecommendations, 4),
            AverageConfidence = Math.Round(averageConfidence, 4),
            ConversationsLast7Days = conversationsLast7Days,
            RecommendationsLast7Days = recommendationsLast7Days
        };
    }

    public async Task<List<AiTopRecommendedServiceDto>> GetTopRecommendedServicesAsync(
        int take = 10,
        CancellationToken cancellationToken = default)
    {
        return await _context.AiServiceRecommendations
            .AsNoTracking()
            .GroupBy(x => new
            {
                x.ServiceId,
                x.ServiceNameSnapshot
            })
            .Select(g => new AiTopRecommendedServiceDto
            {
                ServiceId = g.Key.ServiceId,
                ServiceNameSnapshot = g.Key.ServiceNameSnapshot,
                RecommendationCount = g.Count(),
                AverageConfidence = Math.Round(g.Average(x => x.Confidence), 4),
                HelpfulCount = g.Count(x => x.FeedbackValue == AiRecommendationFeedbackValue.Helpful),
                NotHelpfulCount = g.Count(x => x.FeedbackValue == AiRecommendationFeedbackValue.NotHelpful)
            })
            .OrderByDescending(x => x.RecommendationCount)
            .ThenByDescending(x => x.AverageConfidence)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AiDailyUsageDto>> GetDailyUsageAsync(
        int days = 14,
        CancellationToken cancellationToken = default)
    {
        var startDate = DateTime.UtcNow.Date.AddDays(-(days - 1));

        var conversations = await _context.AiConversations
            .AsNoTracking()
            .Where(x => x.CreatedAt >= startDate)
            .GroupBy(x => x.CreatedAt.Date)
            .Select(g => new
            {
                Date = g.Key,
                Count = g.Count()
            })
            .ToListAsync(cancellationToken);

        var messages = await _context.AiConversationMessages
            .AsNoTracking()
            .Where(x => x.CreatedAt >= startDate)
            .GroupBy(x => x.CreatedAt.Date)
            .Select(g => new
            {
                Date = g.Key,
                Count = g.Count()
            })
            .ToListAsync(cancellationToken);

        var recommendations = await _context.AiServiceRecommendations
            .AsNoTracking()
            .Where(x => x.CreatedAt >= startDate)
            .GroupBy(x => x.CreatedAt.Date)
            .Select(g => new
            {
                Date = g.Key,
                Count = g.Count()
            })
            .ToListAsync(cancellationToken);

        var conversationMap = conversations.ToDictionary(x => DateOnly.FromDateTime(x.Date), x => x.Count);
        var messageMap = messages.ToDictionary(x => DateOnly.FromDateTime(x.Date), x => x.Count);
        var recommendationMap = recommendations.ToDictionary(x => DateOnly.FromDateTime(x.Date), x => x.Count);

        var result = new List<AiDailyUsageDto>();

        for (var i = 0; i < days; i++)
        {
            var date = DateOnly.FromDateTime(startDate.AddDays(i));

            result.Add(new AiDailyUsageDto
            {
                Date = date,
                Conversations = conversationMap.GetValueOrDefault(date),
                Messages = messageMap.GetValueOrDefault(date),
                Recommendations = recommendationMap.GetValueOrDefault(date)
            });
        }

        return result;
    }
}