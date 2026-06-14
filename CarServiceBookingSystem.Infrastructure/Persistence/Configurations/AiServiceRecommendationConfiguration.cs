using CarServiceBookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarServiceBookingSystem.Infrastructure.Configurations;

public sealed class AiServiceRecommendationConfiguration : IEntityTypeConfiguration<AiServiceRecommendation>
{
    public void Configure(EntityTypeBuilder<AiServiceRecommendation> builder)
    {
        builder.ToTable("AiServiceRecommendations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ServiceNameSnapshot)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Reason)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.BookingUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.AssistantMessage)
            .WithMany()
            .HasForeignKey(x => x.AssistantMessageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ConversationId);

        builder.HasIndex(x => x.ServiceId);
    }
}