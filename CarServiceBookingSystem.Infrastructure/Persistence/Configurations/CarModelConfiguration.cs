using CarServiceBookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Infrastructure.Persistence.Configurations
{
    internal class CarModelConfiguration : IEntityTypeConfiguration<CarModel>
    {
        public void Configure(EntityTypeBuilder<CarModel> builder)
        {
            var tableName = builder.Metadata.GetTableName();
            builder.ToTable(tb => tb.IsTemporal(t =>
            {
                t.HasPeriodStart("PeriodStart");
                t.HasPeriodEnd("PeriodEnd");
                t.UseHistoryTable($"{tableName}History", schema: "auditing");
            }));

            builder.HasMany(x => x.Years)
            .WithOne(x => x.Model)
            .HasForeignKey(x => x.ModelId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
