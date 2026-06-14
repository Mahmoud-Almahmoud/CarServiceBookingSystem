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

    public async Task<List<AiConversationSummaryDto>> GetUserConversationsAsync(
    string userId,
    bool includeArchived = false,
    CancellationToken cancellationToken = default)
    {
        var conversations = await _context.AiConversations
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Where(x => includeArchived || !x.IsArchived)
            .OrderByDescending(x => x.UpdatedAt)
            .Select(x => new AiConversationSummaryDto
            {
                Id = x.Id,
                CarId = x.CarId,
                Title = x.Title,
                IsArchived = x.IsArchived,
                MessagesCount = x.Messages.Count,
                LastMessagePreview = x.Messages
                    .OrderByDescending(m => m.CreatedAt)
                    .Select(m => m.Content)
                    .FirstOrDefault(),
                CreatedAtUtc = x.CreatedAt,
                UpdatedAtUtc = x.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return conversations
            .Select(x => new AiConversationSummaryDto
            {
                Id = x.Id,
                CarId = x.CarId,
                Title = x.Title,
                IsArchived = x.IsArchived,
                MessagesCount = x.MessagesCount,
                LastMessagePreview = TrimPreview(x.LastMessagePreview),
                CreatedAtUtc = x.CreatedAtUtc,
                UpdatedAtUtc = x.UpdatedAtUtc
            })
            .ToList();
    }

    public async Task<AiConversationDetailsDto?> GetConversationDetailsAsync(
        string userId,
        int conversationId,
        CancellationToken cancellationToken = default)
    {
        var conversation = await _context.AiConversations
            .AsNoTracking()
            .Where(x => x.Id == conversationId &&
                        x.UserId == userId &&
                        !x.IsArchived)
            .Select(x => new AiConversationDetailsDto
            {
                Id = x.Id,
                UserId = x.UserId,
                CarId = x.CarId,
                Title = x.Title,
                IsArchived = x.IsArchived,
                CreatedAtUtc = x.CreatedAt,
                UpdatedAtUtc = x.UpdatedAt,
                Messages = x.Messages
                    .OrderBy(m => m.CreatedAt)
                    .Select(m => new AiConversationMessageDto
                    {
                        Id = m.Id,
                        ConversationId = m.ConversationId,
                        Role = m.Role.ToString(),
                        Content = m.Content,
                        CreatedAtUtc = m.CreatedAt
                    })
                    .ToList(),
                Recommendations = x.Recommendations
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new AiServiceRecommendationHistoryDto
                    {
                        Id = r.Id,
                        ConversationId = r.ConversationId,
                        AssistantMessageId = r.AssistantMessageId,
                        ServiceId = r.ServiceId,
                        ServiceNameSnapshot = r.ServiceNameSnapshot,
                        Reason = r.Reason,
                        Confidence = r.Confidence,
                        BookingUrl = r.BookingUrl,
                        CreatedAtUtc = r.CreatedAt
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return conversation;
    }

    public async Task<bool> ArchiveConversationAsync(
        string userId,
        int conversationId,
        CancellationToken cancellationToken = default)
    {
        var affectedRows = await _context.AiConversations
            .Where(x => x.Id == conversationId &&
                        x.UserId == userId &&
                        !x.IsArchived)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.IsArchived, true)
                .SetProperty(x => x.UpdatedAt, DateTime.UtcNow),
                cancellationToken);

        return affectedRows > 0;
    }

    private static string? TrimPreview(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var trimmed = value.Trim();

        return trimmed.Length <= 160
            ? trimmed
            : trimmed[..160] + "...";
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