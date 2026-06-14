using CarServiceBookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarServiceBookingSystem.Infrastructure.Configurations;

public sealed class AiAdvisorSettingConfiguration : IEntityTypeConfiguration<AiAdvisorSetting>
{
    public void Configure(EntityTypeBuilder<AiAdvisorSetting> builder)
    {
        builder.ToTable("AiAdvisorSettings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Provider)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.BaseUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Model)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.BookingPathTemplate)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.UpdatedByUserId)
            .HasMaxLength(450);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .IsRequired();

        builder.HasData(new AiAdvisorSetting
        {
            Id = 1,
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
            CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}