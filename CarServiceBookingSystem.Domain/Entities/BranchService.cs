namespace CarServiceBookingSystem.Domain.Entities
{
    public class BranchService : BaseEntity
    {
        public int ServiceBranchId { get; set; }

        public ServiceBranch ServiceBranch { get; set; } = null!;

        public int ServiceId { get; set; }

        public Service Service { get; set; } = null!;

        public bool IsActive { get; set; } = true;
    }
}