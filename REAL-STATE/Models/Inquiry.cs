namespace REAL_STATE.Models
{
    public class Inquiry
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string PropertyId { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public string? AgentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Status { get; set; } = "New";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Property Property { get; set; } = null!;
        public ApplicationUser? User { get; set; }
        public Agent? Agent { get; set; }
    }
   
}
