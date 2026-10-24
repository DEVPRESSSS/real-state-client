namespace REAL_STATE.Models.VM
{
    public class AgentCardVM
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = "Agent";
        public string? PhotoUrl { get; set; }
        public string? Email { get; set; }
        public bool IsVerified { get; set; }
        public string Area { get; set; } = "-";
        public int ActiveListings { get; set; }
    }

    public class PagedAgentsVM
    {
        public List<AgentCardVM> Agents { get; set; } = new();
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
    }
}