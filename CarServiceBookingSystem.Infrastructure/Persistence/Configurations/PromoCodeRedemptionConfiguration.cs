using CarServiceBookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarServiceBookingSystem.Infrastructure.Persistence.Configurations;

public class PromoCodeRedemptionConfiguration : IEntityTypeConfiguration<PromoCodeRedemption>
{
    public void Configure(EntityTypeBuilder<PromoCodeRedemption> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(x => x.DiscountAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.PromoCode)
            .WithMany(x => x.Redemptions)
            .HasForeignKey(x => x.PromoCodeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Booking)
            .WithOne(x => x.PromoCodeRedemption)
            .HasForeignKey<PromoCodeRedemption>(x => x.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.PromoCodeId);

        builder.HasIndex(x => new
        {
            x.PromoCodeId,
            x.UserId
        });

        builder.HasIndex(x => x.BookingId)
            .IsUnique();
    }
}