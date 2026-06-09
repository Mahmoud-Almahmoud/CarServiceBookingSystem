using CarServiceBookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarServiceBookingSystem.Infrastructure.Persistence.Configurations;

public class IdempotencyKeyConfiguration : IEntityTypeConfiguration<IdempotencyKey>
{
    public void Configure(EntityTypeBuilder<IdempotencyKey> builder)
    {
        var tableName = builder.Metadata.GetTableName();
        builder.ToTable(tb => tb.IsTemporal(t =>
        {
            t.HasPeriodStart("PeriodStart");
            t.HasPeriodEnd("PeriodEnd");
            t.UseHistoryTable($"{tableName}History", schema: "auditing");
        }));

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Key)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(x => x.Endpoint)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(x => x.RequestHash)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.ResponseBody)
            .HasColumnType("nvarchar(max)");

        builder.HasIndex(x => new { x.UserId, x.Key })
            .IsUnique();

        builder.HasIndex(x => x.ExpiresAtUtc);
    }
}