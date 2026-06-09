using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Infrastructure.Persistence.Configurations
{
    internal class BranchWorkingHourConfiguration : IEntityTypeConfiguration<Domain.Entities.BranchWorkingHour>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Domain.Entities.BranchWorkingHour> builder)
        {
            var tableName = builder.Metadata.GetTableName();
            builder.ToTable(tb => tb.IsTemporal(t =>
            {
                t.HasPeriodStart("PeriodStart");
                t.HasPeriodEnd("PeriodEnd");
                t.UseHistoryTable($"{tableName}History", schema: "auditing");
            }));

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DayOfWeek)
                .IsRequired();

            builder.Property(x => x.OpenTime)
                .IsRequired();

            builder.Property(x => x.CloseTime)
                .IsRequired();

            builder.Property(x => x.IsClosed)
                .IsRequired();

            builder.HasOne(x => x.ServiceBranch)
                .WithMany(x => x.WorkingHours)
                .HasForeignKey(x => x.ServiceBranchId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new
            {
                x.ServiceBranchId,
                x.DayOfWeek
            }).IsUnique();
        }
    }
}
