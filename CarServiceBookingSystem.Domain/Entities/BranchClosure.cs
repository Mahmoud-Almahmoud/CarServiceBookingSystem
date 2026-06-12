using CarServiceBookingSystem.Domain.Enums;

namespace CarServiceBookingSystem.Domain.Entities
{
    public class BranchClosure : BaseIdEntity
    {
        public int ServiceBranchId { get; set; }

        public ServiceBranch ServiceBranch { get; set; } = null!;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsFullDay { get; set; } = true;

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public BranchClosureType Type { get; set; }

        public string Reason { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}