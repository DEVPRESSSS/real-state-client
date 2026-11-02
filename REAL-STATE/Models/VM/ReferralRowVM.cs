namespace REAL_STATE.Models.VM
{
    public class ReferralRowVM
    {
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? ReferredName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}