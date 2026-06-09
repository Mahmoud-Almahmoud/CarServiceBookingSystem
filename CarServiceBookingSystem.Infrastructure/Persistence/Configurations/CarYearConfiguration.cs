using CarServiceBookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Infrastructure.Persistence.Configurations
{
    internal class CarYearConfiguration : IEntityTypeConfiguration<CarYear>
    {
        public void Configure(EntityTypeBuilder<CarYear> builder)
        {
            var tableName = builder.Metadata.GetTableName();
            builder.ToTable(tb => tb.IsTemporal(t =>
            {
                t.HasPeriodStart("PeriodStart");
                t.HasPeriodEnd("PeriodEnd");
                t.UseHistoryTable($"{tableName}History", schema: "auditing");
            }));

            builder.HasMany(x => x.Trims)
            .WithOne(x => x.Year)
            .HasForeignKey(x => x.YearId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
