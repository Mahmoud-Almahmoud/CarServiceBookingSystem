using CarServiceBookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Infrastructure.Persistence.Configurations
{
    internal class ServiceBranchConfiguration : IEntityTypeConfiguration<Domain.Entities.ServiceBranch>
    {
        public void Configure(EntityTypeBuilder<ServiceBranch> builder)
        {
            var tableName = builder.Metadata.GetTableName();
            builder.ToTable(tb => tb.IsTemporal(t =>
            {
                t.HasPeriodStart("PeriodStart");
                t.HasPeriodEnd("PeriodEnd");
                t.UseHistoryTable($"{tableName}History", schema: "auditing");
            }));

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.CountryCode)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(x => x.City)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Latitude)
                .HasPrecision(10, 7);

            builder.Property(x => x.Longitude)
                .HasPrecision(10, 7);

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.HasIndex(x => x.Name);

            builder.HasIndex(x => new
            {
                x.CountryCode,
                x.City,
                x.IsActive
            });
        }
    }
}
