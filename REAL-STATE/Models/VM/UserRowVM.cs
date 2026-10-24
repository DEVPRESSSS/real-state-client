namespace REAL_STATE.Models.VM
{
    public class UserRowVM
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = "-";
        public bool IsLocked { get; set; }
        public bool IsCurrentUser { get; set; }
    }
}
