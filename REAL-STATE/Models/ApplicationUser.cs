using Microsoft.AspNetCore.Identity;

namespace REAL_STATE.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();
        public ICollection<Inquiry> Inquiries { get; set; } = new List<Inquiry>();
        public ICollection<ViewingAppointment> ViewingAppointments { get; set; } = new List<ViewingAppointment>();
        public ICollection<Agent> AgentProfiles { get; set; } = new List<Agent>();
    }
}
