namespace REAL_STATE.Models.VM
{
    public class DeveloperCardVM
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? Description { get; set; }
        public string? WebsiteUrl { get; set; }
        public int AvailableListings { get; set; }
    }
}