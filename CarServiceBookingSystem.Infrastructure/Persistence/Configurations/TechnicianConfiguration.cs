using CarServiceBookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarServiceBookingSystem.Infrastructure.Persistence.Configurations;

public class TechnicianConfiguration : IEntityTypeConfiguration<Technician>
{
    public void Configure(EntityTypeBuilder<Technician> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(30);

        builder.Property(x => x.Email)
            .HasMaxLength(150);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.ServiceBranch)
            .WithMany(x => x.Technicians)
            .HasForeignKey(x => x.ServiceBranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Bookings)
            .WithOne(x => x.Technician)
            .HasForeignKey(x => x.TechnicianId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ServiceBranchId);

        builder.HasIndex(x => new { x.ServiceBranchId, x.IsActive });
    }
}