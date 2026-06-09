using Microsoft.EntityFrameworkCore;
using CarServiceBookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarServiceBookingSystem.Infrastructure.Persistence.Configurations
{
    public class StripWebhookEventConfiguration : IEntityTypeConfiguration<StripeWebhookEvent>
    {
        public void Configure(EntityTypeBuilder<StripeWebhookEvent> builder)
        {
            var tableName = builder.Metadata.GetTableName();
            builder.ToTable(tb => tb.IsTemporal(t =>
            {
                t.HasPeriodStart("PeriodStart");
                t.HasPeriodEnd("PeriodEnd");
                t.UseHistoryTable($"{tableName}History", schema: "auditing");
            }));

            builder.HasKey(x => x.Id);

            builder.Property(x => x.StripeEventId)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.EventType)
                .HasMaxLength(200)
                .IsRequired();

            builder.HasIndex(x => x.StripeEventId)
                .IsUnique();
        }
    }
}
