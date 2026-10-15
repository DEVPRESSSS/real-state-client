namespace REAL_STATE.Models
{
    public class Agent
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? PhotoUrl { get; set; }
        public string? Bio { get; set; }
        public string? LicenseNumber { get; set; }
        public string Role { get; set; } = "Agent";
        public bool IsActive { get; set; } = true;

        public ApplicationUser? User { get; set; }
        public ICollection<Property> Properties { get; set; } = new List<Property>();
        public ICollection<Inquiry> Inquiries { get; set; } = new List<Inquiry>();
        public ICollection<ViewingAppointment> ViewingAppointments { get; set; } = new List<ViewingAppointment>();
    }
}
