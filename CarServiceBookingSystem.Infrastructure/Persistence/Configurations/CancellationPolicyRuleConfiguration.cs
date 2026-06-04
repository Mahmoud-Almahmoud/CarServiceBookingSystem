using CarServiceBookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarServiceBookingSystem.Infrastructure.Persistence.Configurations;

public class CancellationPolicyRuleConfiguration : IEntityTypeConfiguration<CancellationPolicyRule>
{
    public void Configure(EntityTypeBuilder<CancellationPolicyRule> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.HoursBeforeStart)
            .IsRequired();

        builder.Property(x => x.RefundPercentage)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(300);

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

        builder.HasIndex(x => new
        {
            x.ServiceId,
            x.ServiceBranchId,
            x.HoursBeforeStart,
            x.IsActive
        });
    }
}