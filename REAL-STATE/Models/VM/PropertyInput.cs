using System.ComponentModel.DataAnnotations;

namespace REAL_STATE.Models.VM;

// Only editable scalar fields belong in a form. Never bind an EF navigation graph.
public sealed class PropertyInput
{
    [StringLength(36)] public string Id { get; set; } = Guid.NewGuid().ToString();
    [Required, StringLength(36)] public string DeveloperId { get; set; } = "";
    [StringLength(36)] public string? AgentId { get; set; }
    [Required, StringLength(200)] public string Title { get; set; } = "";
    [Required, StringLength(250), RegularExpression("[a-z0-9]+(?:-[a-z0-9]+)*")]
    public string Slug { get; set; } = "";
    [StringLength(8000)] public string? Description { get; set; }
    [Required, StringLength(50)] public string PropertyType { get; set; } = "";
    [Required, RegularExpression("Sale|Rent")] public string ListingType { get; set; } = "Sale";
    [Range(typeof(decimal), "0", "9999999999999999.99")] public decimal Price { get; set; }
    [Range(typeof(decimal), "0", "9999999999999999.99")] public decimal FloorAreaSqFt { get; set; }
    [Range(0, 1000)] public int? Bedrooms { get; set; }
    [Range(0, 1000)] public int? Bathrooms { get; set; }
    [StringLength(300)] public string? Address { get; set; }
    [Required, StringLength(100)] public string City { get; set; } = "";
    [StringLength(100)] public string? Province { get; set; }
    [Required, StringLength(100)] public string Country { get; set; } = "Philippines";
    [Required, RegularExpression("Available|Sold|Rented|Reserved|Unavailable")]
    public string Status { get; set; } = "Available";
    public bool IsFeatured { get; set; }

    public static PropertyInput FromEntity(Property property) => new()
    {
        Id = property.Id, DeveloperId = property.DeveloperId, AgentId = property.AgentId,
        Title = property.Title, Slug = property.Slug, Description = property.Description,
        PropertyType = property.PropertyType, ListingType = property.ListingType,
        Price = property.Price, FloorAreaSqFt = property.FloorAreaSqFt,
        Bedrooms = property.Bedrooms, Bathrooms = property.Bathrooms, Address = property.Address,
        City = property.City, Province = property.Province, Country = property.Country,
        Status = property.Status, IsFeatured = property.IsFeatured
    };

    public void ApplyTo(Property property)
    {
        property.DeveloperId = DeveloperId;
        property.AgentId = string.IsNullOrWhiteSpace(AgentId) ? null : AgentId;
        property.Title = Title.Trim();
        property.Slug = Slug.Trim();
        property.Description = Description;
        property.PropertyType = PropertyType.Trim();
        property.ListingType = ListingType;
        property.Price = Price;
        property.FloorAreaSqFt = FloorAreaSqFt;
        property.Bedrooms = Bedrooms;
        property.Bathrooms = Bathrooms;
        property.Address = Address;
        property.City = City.Trim();
        property.Province = Province;
        property.Country = Country.Trim();
        property.Status = Status;
        property.IsFeatured = IsFeatured;
    }
}
