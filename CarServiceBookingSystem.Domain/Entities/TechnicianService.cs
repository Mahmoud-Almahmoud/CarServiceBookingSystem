namespace CarServiceBookingSystem.Domain.Entities;

public class TechnicianService :BaseIdEntity
{
    public int TechnicianId { get; set; }
    public Technician Technician { get; set; } = null!;

    public int ServiceId { get; set; }
    public Service Service { get; set; } = null!;

}