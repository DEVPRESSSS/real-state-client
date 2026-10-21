namespace REAL_STATE.Models
{
 
    public class Property
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string DeveloperId { get; set; } = string.Empty;
        public string? AgentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string PropertyType { get; set; } = string.Empty;
        public string ListingType { get; set; } = "Sale";
        public decimal Price { get; set; }
        public decimal FloorAreaSqFt { get; set; }
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public string? Address { get; set; }
        public string City { get; set; } = string.Empty;
        public string? Province { get; set; }
        public string Country { get; set; } = "Philippines";
        public string Status { get; set; } = "Available";
        public bool IsFeatured { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public Developers? Developer { get; set; } = null!;
        public Agent? Agent { get; set; }
        public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
        public ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();
        public ICollection<Inquiry> Inquiries { get; set; } = new List<Inquiry>();
        public ICollection<ViewingAppointment> ViewingAppointments { get; set; } = new List<ViewingAppointment>();
    }


}
