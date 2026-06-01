using CarServiceBookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarServiceBookingSystem.Infrastructure.Persistence.Configurations;

public class TechnicianServiceConfiguration : IEntityTypeConfiguration<TechnicianService>
{
    public void Configure(EntityTypeBuilder<TechnicianService> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Technician)
            .WithMany(x => x.TechnicianServices)
            .HasForeignKey(x => x.TechnicianId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Service)
            .WithMany()
            .HasForeignKey(x => x.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.TechnicianId, x.ServiceId })
            .IsUnique();
    }
}