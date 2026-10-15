namespace REAL_STATE.Models
{

    public class ViewingAppointment
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string PropertyId { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public string? AgentId { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string? ContactPhone { get; set; }
        public DateTime ScheduledAt { get; set; }
        public string Status { get; set; } = "Pending";
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Property Property { get; set; } = null!;
        public ApplicationUser? User { get; set; }
        public Agent? Agent { get; set; }
    }
}
