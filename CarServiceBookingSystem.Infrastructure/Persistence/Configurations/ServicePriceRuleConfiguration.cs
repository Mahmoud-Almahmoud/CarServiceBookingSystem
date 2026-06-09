using CarServiceBookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Infrastructure.Persistence.Configurations
{
    internal class ServicePriceRuleConfiguration : IEntityTypeConfiguration<ServicePriceRule>
    {
        public void Configure(EntityTypeBuilder<ServicePriceRule> builder)
        {
            var tableName = builder.Metadata.GetTableName();
            builder.ToTable(tb => tb.IsTemporal(t =>
            {
                t.HasPeriodStart("PeriodStart");
                t.HasPeriodEnd("PeriodEnd");
                t.UseHistoryTable($"{tableName}History", schema: "auditing");
            }));

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.DurationMinutes)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.HasOne(x => x.Service)
                .WithMany(x => x.PriceRules)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.CarBrand)
                .WithMany()
                .HasForeignKey(x => x.CarBrandId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CarModel)
                .WithMany()
                .HasForeignKey(x => x.CarModelId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CarTrim)
                .WithMany()
                .HasForeignKey(x => x.CarTrimId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.ServiceId);

            builder.HasIndex(x => new
            {
                x.ServiceId,
                x.CarBrandId,
                x.CarModelId,
                x.CarTrimId,
                x.CarYearId,
                x.IsActive
            });
        }
    }
}
