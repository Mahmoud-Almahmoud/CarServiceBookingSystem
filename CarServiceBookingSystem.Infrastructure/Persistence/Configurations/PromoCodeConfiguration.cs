using CarServiceBookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarServiceBookingSystem.Infrastructure.Persistence.Configurations;

public class PromoCodeConfiguration : IEntityTypeConfiguration<PromoCode>
{
    public void Configure(EntityTypeBuilder<PromoCode> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Description)
            .HasMaxLength(300);

        builder.Property(x => x.DiscountType)
            .IsRequired();

        builder.Property(x => x.DiscountValue)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.MaxDiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.MinimumSubtotalAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.Service)
            .WithMany()
            .HasForeignKey(x => x.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ServiceBranch)
            .WithMany()
            .HasForeignKey(x => x.ServiceBranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.HasIndex(x => new
        {
            x.ServiceId,
            x.ServiceBranchId,
            x.IsActive
        });
    }
}