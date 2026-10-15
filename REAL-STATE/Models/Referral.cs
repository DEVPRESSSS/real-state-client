namespace REAL_STATE.Models
{
    public class Referral
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ReferrerUserId { get; set; } = string.Empty;
        public string? ReferredUserId { get; set; }
        public string ReferralCode { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ApplicationUser ReferrerUser { get; set; } = null!;
        public ApplicationUser? ReferredUser { get; set; }
    }
    
}
