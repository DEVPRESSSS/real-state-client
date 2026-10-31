namespace REAL_STATE.Models.VM
{
    public class AppointmentListVM
    {
        
        public string Id { get; set; } = string.Empty;

        public string Property { get; set; } = string.Empty;

        public string ContactName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string Agent { get; set; } = "Unassigned";

        public DateTime ScheduledAt { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
