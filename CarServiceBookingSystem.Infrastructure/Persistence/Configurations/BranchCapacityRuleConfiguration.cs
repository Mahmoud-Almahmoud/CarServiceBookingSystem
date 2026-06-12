using CarServiceBookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Infrastructure.Persistence.Configurations
{
    internal class BranchCapacityRuleConfiguration : IEntityTypeConfiguration<BranchCapacityRule>
    {
        public void Configure(EntityTypeBuilder<BranchCapacityRule> builder)
        {
            var tableName = builder.Metadata.GetTableName();
            builder.ToTable(tb => tb.IsTemporal(t =>
            {
                t.HasPeriodStart("PeriodStart");
                t.HasPeriodEnd("PeriodEnd");
                t.UseHistoryTable($"{tableName}History", schema: "auditing");
            }));

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Capacity)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.HasOne(x => x.ServiceBranch)
                .WithMany(x => x.CapacityRules)
                .HasForeignKey(x => x.ServiceBranchId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new
            {
                x.ServiceBranchId,
                x.DayOfWeek,
                x.StartTime,
                x.EndTime,
                x.IsActive
            });
        }
    }
}
