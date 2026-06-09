using CarServiceBookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Infrastructure.Persistence.Configurations
{
    internal class ServiceConfiguration : IEntityTypeConfiguration<Domain.Entities.Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            var tableName = builder.Metadata.GetTableName();
            builder.ToTable(tb => tb.IsTemporal(t =>
            {
                t.HasPeriodStart("PeriodStart");
                t.HasPeriodEnd("PeriodEnd");
                t.UseHistoryTable($"{tableName}History", schema: "auditing");
            }));

            builder.HasMany<Booking>()
            .WithOne(x => x.Service)
            .HasForeignKey(x => x.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Price)
            .HasColumnType("decimal(18,2)");
        }
    }
}
