using CarServiceBookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Infrastructure.Persistence.Configurations
{
    public class ServiceAreaRuleConfiguration : IEntityTypeConfiguration<ServiceAreaRule>
    {
        public void Configure(EntityTypeBuilder<ServiceAreaRule> builder)
        {
            var tableName = builder.Metadata.GetTableName();
            builder.ToTable(tb => tb.IsTemporal(t =>
            {
                t.HasPeriodStart("PeriodStart");
                t.HasPeriodEnd("PeriodEnd");
                t.UseHistoryTable($"{tableName}History", schema: "auditing");
            }));

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CountryCode)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(x => x.City)
                .HasMaxLength(100);

            builder.Property(x => x.IsAllowed)
                .IsRequired();

            builder.Property(x => x.Priority)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.HasOne(x => x.Service)
                .WithMany()
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new
            {
                x.ServiceId,
                x.CountryCode,
                x.City,
                x.IsActive
            });

            builder.HasIndex(x => x.Priority);
        }
    }
}
