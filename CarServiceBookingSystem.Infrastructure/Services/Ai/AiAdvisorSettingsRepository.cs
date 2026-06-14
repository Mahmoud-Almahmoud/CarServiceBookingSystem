using CarServiceBookingSystem.Application.DTOs.Ai;
using CarServiceBookingSystem.Application.Interfaces.IAi;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CarServiceBookingSystem.Infrastructure.Services.Ai;

public sealed class AiAdvisorSettingsRepository : IAiAdvisorSettingsRepository
{
    private const int SettingsId = 1;

    private readonly ApplicationDbContext _context;

    public AiAdvisorSettingsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AiAdvisorSettingsDto> GetAsync(
        CancellationToken cancellationToken = default)
    {
        var settings = await GetOrCreateEntityAsync(cancellationToken);

        return Map(settings);
    }

    public async Task<AiAdvisorSettingsDto> UpdateAsync(
        UpdateAiAdvisorSettingsRequest request,
        string? updatedByUserId,
        CancellationToken cancellationToken = default)
    {
        var settings = await GetOrCreateEntityAsync(cancellationToken);

        settings.Enabled = request.Enabled;
        settings.Provider = request.Provider.Trim();
        settings.BaseUrl = request.BaseUrl.Trim().TrimEnd('/');
        settings.Model = request.Model.Trim();
        settings.TimeoutSeconds = request.TimeoutSeconds;
        settings.MaxSuggestions = request.MaxSuggestions;
        settings.BookingPathTemplate = request.BookingPathTemplate.Trim();
        settings.MaxPromptLength = request.MaxPromptLength;
        settings.MinimumRecommendationConfidence = request.MinimumRecommendationConfidence;
        settings.BlockUnrelatedQuestions = request.BlockUnrelatedQuestions;
        settings.EnablePromptInjectionFilter = request.EnablePromptInjectionFilter;
        settings.RateLimitPerMinute = request.RateLimitPerMinute;
        settings.UpdatedAtUtc = DateTime.UtcNow;
        settings.UpdatedByUserId = updatedByUserId;

        await _context.SaveChangesAsync(cancellationToken);

        return Map(settings);
    }

    private async Task<AiAdvisorSetting> GetOrCreateEntityAsync(
        CancellationToken cancellationToken)
    {
        var settings = await _context.AiAdvisorSettings
            .FirstOrDefaultAsync(x => x.Id == SettingsId, cancellationToken);

        if (settings is not null)
            return settings;

        settings = new AiAdvisorSetting
        {
            Id = SettingsId,
            Enabled = true,
            Provider = "Ollama",
            BaseUrl = "http://localhost:11434",
            Model = "llama3.1:8b",
            TimeoutSeconds = 60,
            MaxSuggestions = 3,
            BookingPathTemplate = "/app/bookings/new?serviceId={0}",
            MaxPromptLength = 2000,
            MinimumRecommendationConfidence = 0.45,
            BlockUnrelatedQuestions = true,
            EnablePromptInjectionFilter = true,
            RateLimitPerMinute = 10,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        _context.AiAdvisorSettings.Add(settings);

        await _context.SaveChangesAsync(cancellationToken);

        return settings;
    }

    private static AiAdvisorSettingsDto Map(AiAdvisorSetting settings)
    {
        return new AiAdvisorSettingsDto
        {
            Enabled = settings.Enabled,
            Provider = settings.Provider,
            BaseUrl = settings.BaseUrl,
            Model = settings.Model,
            TimeoutSeconds = settings.TimeoutSeconds,
            MaxSuggestions = settings.MaxSuggestions,
            BookingPathTemplate = settings.BookingPathTemplate,
            MaxPromptLength = settings.MaxPromptLength,
            MinimumRecommendationConfidence = settings.MinimumRecommendationConfidence,
            BlockUnrelatedQuestions = settings.BlockUnrelatedQuestions,
            EnablePromptInjectionFilter = settings.EnablePromptInjectionFilter,
            RateLimitPerMinute = settings.RateLimitPerMinute,
            CreatedAtUtc = settings.CreatedAtUtc,
            UpdatedAtUtc = settings.UpdatedAtUtc,
            UpdatedByUserId = settings.UpdatedByUserId
        };
    }
}