namespace CarServiceBookingSystem.Domain.Entities
{
    public class BranchCapacityRule : BaseEntity
    {
        public int ServiceBranchId { get; set; }

        public ServiceBranch ServiceBranch { get; set; } = null!;

        public DayOfWeek? DayOfWeek { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public int Capacity { get; set; }

        public bool IsActive { get; set; } = true;
    }
}