using CarServiceBookingSystem.Application.DTOs.Ai;
using CarServiceBookingSystem.Application.Interfaces.IAi;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Domain.Enums;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CarServiceBookingSystem.Infrastructure.Ai;

public sealed class AiConversationRepository : IAiConversationRepository
{
    private readonly ApplicationDbContext _context;

    public AiConversationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AiConversationDto> GetOrCreateConversationAsync(
        string userId,
        int? conversationId,
        int? carId,
        string firstUserMessage,
        CancellationToken cancellationToken = default)
    {
        if (conversationId.HasValue)
        {
            var existing = await _context.AiConversations
                .AsNoTracking()
                .Where(x => x.Id == conversationId.Value &&
                            x.UserId == userId &&
                            !x.IsArchived)
                .Select(x => new AiConversationDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    CarId = x.CarId,
                    Title = x.Title,
                    IsArchived = x.IsArchived,
                    CreatedAtUtc = x.CreatedAt,
                    UpdatedAtUtc = x.UpdatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (existing is not null)
                return existing;
        }

        var title = BuildConversationTitle(firstUserMessage);

        var conversation = new AiConversation
        {
            UserId = userId,
            CarId = carId,
            Title = title,
            IsArchived = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.AiConversations.Add(conversation);

        await _context.SaveChangesAsync(cancellationToken);

        return new AiConversationDto
        {
            Id = conversation.Id,
            UserId = conversation.UserId,
            CarId = conversation.CarId,
            Title = conversation.Title,
            IsArchived = conversation.IsArchived,
            CreatedAtUtc = conversation.CreatedAt,
            UpdatedAtUtc = conversation.UpdatedAt
        };
    }

    public async Task<AiConversationMessageDto> AddMessageAsync(
        int conversationId,
        string role,
        string content,
        CancellationToken cancellationToken = default)
    {
        var messageRole = ParseRole(role);

        var message = new AiConversationMessage
        {
            ConversationId = conversationId,
            Role = messageRole,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };

        _context.AiConversationMessages.Add(message);

        await _context.AiConversations
            .Where(x => x.Id == conversationId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.UpdatedAt, DateTime.UtcNow),
                cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return new AiConversationMessageDto
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            Role = message.Role.ToString(),
            Content = message.Content,
            CreatedAtUtc = message.CreatedAt
        };
    }

    public async Task<List<AiConversationMessageDto>> GetRecentMessagesAsync(
        int conversationId,
        int take = 10,
        CancellationToken cancellationToken = default)
    {
        return await _context.AiConversationMessages
            .AsNoTracking()
            .Where(x => x.ConversationId == conversationId)
            .OrderByDescending(x => x.CreatedAt)
            .Take(take)
            .OrderBy(x => x.CreatedAt)
            .Select(x => new AiConversationMessageDto
            {
                Id = x.Id,
                ConversationId = x.ConversationId,
                Role = x.Role.ToString(),
                Content = x.Content,
                CreatedAtUtc = x.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task AddRecommendationsAsync(
        int conversationId,
        int assistantMessageId,
        List<CreateAiRecommendationDto> recommendations,
        CancellationToken cancellationToken = default)
    {
        if (recommendations.Count == 0)
            return;

        var entities = recommendations.Select(x => new AiServiceRecommendation
        {
            ConversationId = conversationId,
            AssistantMessageId = assistantMessageId,
            ServiceId = x.ServiceId,
            ServiceNameSnapshot = x.ServiceNameSnapshot,
            Reason = x.Reason,
            Confidence = x.Confidence,
            BookingUrl = x.BookingUrl,
            CreatedAt = DateTime.UtcNow
        });

        _context.AiServiceRecommendations.AddRange(entities);

        await _context.SaveChangesAsync(cancellationToken);
    }

    private static AiMessageRole ParseRole(string role)
    {
        if (Enum.TryParse<AiMessageRole>(role, ignoreCase: true, out var parsed))
            return parsed;

        return AiMessageRole.User;
    }

    private static string BuildConversationTitle(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return "Car service advisor chat";

        var normalized = message.Trim();

        return normalized.Length <= 80
            ? normalized
            : normalized[..80];
    }
}