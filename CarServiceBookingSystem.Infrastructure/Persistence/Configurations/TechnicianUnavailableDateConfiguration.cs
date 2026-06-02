using CarServiceBookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarServiceBookingSystem.Infrastructure.Persistence.Configurations;

public class TechnicianUnavailableDateConfiguration : IEntityTypeConfiguration<TechnicianUnavailableDate>
{
    public void Configure(EntityTypeBuilder<TechnicianUnavailableDate> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Reason)
            .HasMaxLength(300);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.Technician)
            .WithMany(x => x.UnavailableDates)
            .HasForeignKey(x => x.TechnicianId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new
        {
            x.TechnicianId,
            x.StartDate,
            x.EndDate,
            x.IsActive
        });
    }
}