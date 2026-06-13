using CarServiceBookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarServiceBookingSystem.Infrastructure.Persistence.Configurations;

public class PushSubscriptionConfiguration : IEntityTypeConfiguration<PushSubscription>
{
    public void Configure(EntityTypeBuilder<PushSubscription> builder)
    {
        builder.ToTable("PushSubscriptions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(x => x.Endpoint)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.P256dh)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Auth)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.UserAgent)
            .HasMaxLength(500);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => x.Endpoint)
            .IsUnique();

        builder.HasIndex(x => new { x.UserId, x.IsActive });
    }
}