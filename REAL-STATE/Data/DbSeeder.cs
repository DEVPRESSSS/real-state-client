using Microsoft.EntityFrameworkCore;
using REAL_STATE.Models;

namespace REAL_STATE.Data
{
    public static class DbSeeder
    {
        private static readonly DateTime SeedDate = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private const string Dev1 = "d0000000-0000-0000-0000-000000000001";
        private const string Dev2 = "d0000000-0000-0000-0000-000000000002";
        private const string Agent1 = "a0000000-0000-0000-0000-000000000001";
        private const string Agent2 = "a0000000-0000-0000-0000-000000000002";
        private const string Prop1 = "p0000000-0000-0000-0000-000000000001";
        private const string Prop2 = "p0000000-0000-0000-0000-000000000002";
        private const string Prop3 = "p0000000-0000-0000-0000-000000000003";
        private const string Prop4 = "p0000000-0000-0000-0000-000000000004";

        public static void Seed(ModelBuilder b)
        {
            b.Entity<Developers>().HasData(
                new Developers
                {
                    Id = Dev1,
                    Name = "Sample Developer One",
                    Slug = "sample-developer-one",
                    Description = "Sample developer for development use.",
                    IsActive = true,
                    CreatedAt = SeedDate
                },
                new Developers
                {
                    Id = Dev2,
                    Name = "Sample Developer Two",
                    Slug = "sample-developer-two",
                    Description = "Sample developer for development use.",
                    IsActive = true,
                    CreatedAt = SeedDate
                });

            b.Entity<Agent>().HasData(
                new Agent
                {
                    Id = Agent1,
                    FullName = "Jerald Montemor",
                    Email = "agent1@example.com",
                    Phone = "+63 900 000 0001",
                    Role = "Agent",
                    IsActive = true
                },
                new Agent
                {
                    Id = Agent2,
                    FullName = "Clive",
                    Email = "broker1@example.com",
                    Phone = "+63 900 000 0002",
                    Role = "Broker",
                    IsActive = true
                });

            b.Entity<Property>().HasData(
                new Property
                {
                    Id = Prop1,
                    DeveloperId = Dev1,
                    AgentId = Agent1,
                    Title = "Sample 2BR Condo in Quezon City",
                    Slug = "sample-2br-condo-quezon-city",
                    Description = "Sample listing for development.",
                    PropertyType = "Condo",
                    ListingType = "Sale",
                    Price = 6500000m,
                    FloorAreaSqFt = 645m,
                    Bedrooms = 2,
                    Bathrooms = 1,
                    Address = "Sample Street",
                    City = "Quezon City",
                    Province = "Metro Manila",
                    Country = "Philippines",
                    Status = "Available",
                    IsFeatured = true,
                    CreatedAt = SeedDate
                },
                new Property
                {
                    Id = Prop2,
                    DeveloperId = Dev1,
                    AgentId = Agent1,
                    Title = "Sample Studio for Rent in Makati",
                    Slug = "sample-studio-rent-makati",
                    PropertyType = "Condo",
                    ListingType = "Rent",
                    Price = 28000m,
                    FloorAreaSqFt = 258m,
                    Bedrooms = 1,
                    Bathrooms = 1,
                    City = "Makati",
                    Province = "Metro Manila",
                    Country = "Philippines",
                    Status = "Available",
                    IsFeatured = false,
                    CreatedAt = SeedDate
                },
                new Property
                {
                    Id = Prop3,
                    DeveloperId = Dev2,
                    AgentId = Agent2,
                    Title = "Sample House and Lot in Cavite",
                    Slug = "sample-house-lot-cavite",
                    PropertyType = "House",
                    ListingType = "Sale",
                    Price = 8900000m,
                    FloorAreaSqFt = 1722m,
                    Bedrooms = 4,
                    Bathrooms = 3,
                    City = "Imus",
                    Province = "Cavite",
                    Country = "Philippines",
                    Status = "Available",
                    IsFeatured = true,
                    CreatedAt = SeedDate
                },
                new Property
                {
                    Id = Prop4,
                    DeveloperId = Dev2,
                    Title = "Sample Lot in Laguna",
                    Slug = "sample-lot-laguna",
                    PropertyType = "Lot",
                    ListingType = "Sale",
                    Price = 3200000m,
                    FloorAreaSqFt = 1614m,
                    City = "Santa Rosa",
                    Province = "Laguna",
                    Country = "Philippines",
                    Status = "Available",
                    IsFeatured = false,
                    CreatedAt = SeedDate
                });

            b.Entity<PropertyImage>().HasData(
                new PropertyImage { Id = "i0000000-0000-0000-0000-000000000001", PropertyId = Prop1, ImageUrl = "/images/properties/placeholder-1.jpg", AltText = "Sample condo", DisplayOrder = 1, IsPrimary = true },
                new PropertyImage { Id = "i0000000-0000-0000-0000-000000000002", PropertyId = Prop2, ImageUrl = "/images/properties/placeholder-2.jpg", AltText = "Sample studio", DisplayOrder = 1, IsPrimary = true },
                new PropertyImage { Id = "i0000000-0000-0000-0000-000000000003", PropertyId = Prop3, ImageUrl = "/images/properties/placeholder-3.jpg", AltText = "Sample house", DisplayOrder = 1, IsPrimary = true },
                new PropertyImage { Id = "i0000000-0000-0000-0000-000000000004", PropertyId = Prop4, ImageUrl = "/images/properties/placeholder-4.jpg", AltText = "Sample lot", DisplayOrder = 1, IsPrimary = true });
        }
    }
}