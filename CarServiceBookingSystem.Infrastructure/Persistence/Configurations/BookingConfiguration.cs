using CarServiceBookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
namespace CarServiceBookingSystem.Infrastructure.Persistence.Configurations
{
    internal class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            var tableName = builder.Metadata.GetTableName();
            builder.ToTable(tb => tb.IsTemporal(t =>
            {
                t.HasPeriodStart("PeriodStart");
                t.HasPeriodEnd("PeriodEnd");
                t.UseHistoryTable($"{tableName}History", schema: "auditing");
            }));

            builder.HasOne(x => x.Payment)
               .WithOne(x => x.Booking)
               .HasForeignKey<Payment>(x => x.BookingId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.ServicePrice)
                .HasPrecision(18, 2);

            builder.Property(x => x.TravelFee)
                .HasPrecision(18, 2);

            builder.Property(x => x.TotalPrice)
                .HasPrecision(18, 2);

            builder.Property(x => x.CustomerLatitude)
                .HasPrecision(10, 7);

            builder.Property(x => x.CustomerLongitude)
                .HasPrecision(10, 7);

            builder.Property(x => x.CustomerCountryCode)
                .HasMaxLength(10);

            builder.Property(x => x.CustomerCity)
                .HasMaxLength(100);

            builder.Property(x => x.DistanceKm)
                .HasPrecision(10, 2);

            builder.HasOne(x => x.ServiceBranch)
                .WithMany(x => x.Bookings)
                .HasForeignKey(x => x.ServiceBranchId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ServiceBranch)
                .WithMany(x => x.Bookings)
                .HasForeignKey(x => x.ServiceBranchId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CancellationReason)
                .HasMaxLength(500);

            builder.Property(x => x.CancelledByUserId)
                .HasMaxLength(450);

            builder.Property(x => x.PromoCodeSnapshot)
                .HasMaxLength(50);

            builder.Property(x => x.SubtotalPrice)
                .HasPrecision(18, 2);

            builder.Property(x => x.DiscountAmount)
                .HasPrecision(18, 2);

            builder.HasOne(x => x.PromoCode)
                .WithMany()
                .HasForeignKey(x => x.PromoCodeId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
