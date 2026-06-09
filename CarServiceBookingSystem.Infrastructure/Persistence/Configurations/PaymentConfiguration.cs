using CarServiceBookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarServiceBookingSystem.Infrastructure.Persistence.Configurations
{
    internal class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            var tableName = builder.Metadata.GetTableName();
            builder.ToTable(tb => tb.IsTemporal(t =>
            {
                t.HasPeriodStart("PeriodStart");
                t.HasPeriodEnd("PeriodEnd");
                t.UseHistoryTable($"{tableName}History", schema: "auditing");
            }));

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Amount)
                .HasPrecision(18, 2);

            builder.Property(x => x.Currency)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(x => x.PaymentIntentId)
                .HasMaxLength(200);

            builder.Property(x => x.StripeClientSecret)
                .HasMaxLength(500);

            builder.Property(x => x.FailureReason)
                .HasMaxLength(1000);

            builder.HasOne(x => x.Booking)
                .WithOne(x => x.Payment)
                .HasForeignKey<Payment>(x => x.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.BookingId)
                 .IsUnique();

            builder.HasIndex(x => x.PaymentIntentId)
                .IsUnique()
                .HasFilter("[PaymentIntentId] IS NOT NULL");

            builder.Property(x => x.StripeRefundId)
                .HasMaxLength(200);

            builder.Property(x => x.RefundedAmount)
                .HasPrecision(18, 2);

            builder.Property(x => x.RefundFailureReason)
                .HasMaxLength(500);
        }
    }
}
