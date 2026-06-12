using CarServiceBookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Infrastructure.Persistence.Configurations
{
    internal class CarTrimConfiguration : IEntityTypeConfiguration<CarTrim>
    {
        public void Configure(EntityTypeBuilder<CarTrim> builder)
        {
            var tableName = builder.Metadata.GetTableName();
            builder.ToTable(tb => tb.IsTemporal(t =>
            {
                t.HasPeriodStart("PeriodStart");
                t.HasPeriodEnd("PeriodEnd");
                t.UseHistoryTable($"{tableName}History", schema: "auditing");
            }));

            builder.HasMany(x => x.Cars)
            .WithOne(x => x.CarTrim)
            .HasForeignKey(x => x.CarTrimId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
