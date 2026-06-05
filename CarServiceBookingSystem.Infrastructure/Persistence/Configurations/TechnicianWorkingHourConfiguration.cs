using CarServiceBookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarServiceBookingSystem.Infrastructure.Persistence.Configurations;

public class TechnicianWorkingHourConfiguration : IEntityTypeConfiguration<TechnicianWorkingHour>
{
    public void Configure(EntityTypeBuilder<TechnicianWorkingHour> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.DayOfWeek)
            .IsRequired();

        builder.Property(x => x.OpenTime)
            .IsRequired();

        builder.Property(x => x.CloseTime)
            .IsRequired();

        builder.Property(x => x.IsClosed)
            .IsRequired();

        builder.HasOne(x => x.Technician)
            .WithMany(x => x.WorkingHours)
            .HasForeignKey(x => x.TechnicianId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.TechnicianId, x.DayOfWeek })
            .IsUnique();
    }
}