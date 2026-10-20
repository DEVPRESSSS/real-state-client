namespace REAL_STATE.Models.DTO.PropertyDTO
{
    namespace REAL_STATE.DTOs
    {
        public class FilterPropertyDTO
        {
            public string Id { get; set; } = string.Empty;
            public string Slug { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public string PropertyType { get; set; } = string.Empty;
            public string ListingType { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public decimal FloorAreaSqFt { get; set; }
            public int? Bedrooms { get; set; }
            public int? Bathrooms { get; set; }
            public string? Address { get; set; }
            public string City { get; set; } = string.Empty;
            public string? Province { get; set; }
            public bool IsFeatured { get; set; }
            public string? DeveloperName { get; set; }
            public string? PrimaryImageUrl { get; set; }
            public string? PrimaryImageAlt { get; set; }
        }
    }
}
