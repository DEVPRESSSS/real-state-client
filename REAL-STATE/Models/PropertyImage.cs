namespace REAL_STATE.Models
{
    public class PropertyImage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string PropertyId { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string? AltText { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsPrimary { get; set; }

        public Property Property { get; set; } = null!;
    }
}
