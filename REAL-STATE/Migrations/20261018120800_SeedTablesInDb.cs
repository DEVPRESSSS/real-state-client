using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace REAL_STATE.Migrations
{
    /// <inheritdoc />
    public partial class SeedTablesInDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Agents",
                columns: new[] { "Id", "Bio", "Email", "FullName", "IsActive", "LicenseNumber", "Phone", "PhotoUrl", "Role", "UserId" },
                values: new object[,]
                {
                    { "a0000000-0000-0000-0000-000000000001", null, "agent1@example.com", "Jerald Montemor", true, null, "+63 900 000 0001", null, "Agent", null },
                    { "a0000000-0000-0000-0000-000000000002", null, "broker1@example.com", "Clive", true, null, "+63 900 000 0002", null, "Broker", null }
                });

            migrationBuilder.InsertData(
                table: "Developers",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "LogoUrl", "Name", "Slug", "WebsiteUrl" },
                values: new object[,]
                {
                    { "d0000000-0000-0000-0000-000000000001", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sample developer for development use.", true, null, "Sample Developer One", "sample-developer-one", null },
                    { "d0000000-0000-0000-0000-000000000002", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sample developer for development use.", true, null, "Sample Developer Two", "sample-developer-two", null }
                });

            migrationBuilder.InsertData(
                table: "Properties",
                columns: new[] { "Id", "Address", "AgentId", "Bathrooms", "Bedrooms", "City", "Country", "CreatedAt", "Description", "DeveloperId", "FloorAreaSqFt", "IsFeatured", "ListingType", "Price", "PropertyType", "Province", "Slug", "Status", "Title", "UpdatedAt" },
                values: new object[] { "p0000000-0000-0000-0000-000000000001", "Sample Street", "a0000000-0000-0000-0000-000000000001", 1, 2, "Quezon City", "Philippines", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sample listing for development.", "d0000000-0000-0000-0000-000000000001", 645m, true, "Sale", 6500000m, "Condo", "Metro Manila", "sample-2br-condo-quezon-city", "Available", "Sample 2BR Condo in Quezon City", null });

            migrationBuilder.InsertData(
                table: "Properties",
                columns: new[] { "Id", "Address", "AgentId", "Bathrooms", "Bedrooms", "City", "Country", "CreatedAt", "Description", "DeveloperId", "FloorAreaSqFt", "ListingType", "Price", "PropertyType", "Province", "Slug", "Status", "Title", "UpdatedAt" },
                values: new object[] { "p0000000-0000-0000-0000-000000000002", null, "a0000000-0000-0000-0000-000000000001", 1, 1, "Makati", "Philippines", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "d0000000-0000-0000-0000-000000000001", 258m, "Rent", 28000m, "Condo", "Metro Manila", "sample-studio-rent-makati", "Available", "Sample Studio for Rent in Makati", null });

            migrationBuilder.InsertData(
                table: "Properties",
                columns: new[] { "Id", "Address", "AgentId", "Bathrooms", "Bedrooms", "City", "Country", "CreatedAt", "Description", "DeveloperId", "FloorAreaSqFt", "IsFeatured", "ListingType", "Price", "PropertyType", "Province", "Slug", "Status", "Title", "UpdatedAt" },
                values: new object[] { "p0000000-0000-0000-0000-000000000003", null, "a0000000-0000-0000-0000-000000000002", 3, 4, "Imus", "Philippines", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "d0000000-0000-0000-0000-000000000002", 1722m, true, "Sale", 8900000m, "House", "Cavite", "sample-house-lot-cavite", "Available", "Sample House and Lot in Cavite", null });

            migrationBuilder.InsertData(
                table: "Properties",
                columns: new[] { "Id", "Address", "AgentId", "Bathrooms", "Bedrooms", "City", "Country", "CreatedAt", "Description", "DeveloperId", "FloorAreaSqFt", "ListingType", "Price", "PropertyType", "Province", "Slug", "Status", "Title", "UpdatedAt" },
                values: new object[] { "p0000000-0000-0000-0000-000000000004", null, null, null, null, "Santa Rosa", "Philippines", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "d0000000-0000-0000-0000-000000000002", 1614m, "Sale", 3200000m, "Lot", "Laguna", "sample-lot-laguna", "Available", "Sample Lot in Laguna", null });

            migrationBuilder.InsertData(
                table: "PropertyImages",
                columns: new[] { "Id", "AltText", "DisplayOrder", "ImageUrl", "IsPrimary", "PropertyId" },
                values: new object[,]
                {
                    { "i0000000-0000-0000-0000-000000000001", "Sample condo", 1, "/images/properties/placeholder-1.jpg", true, "p0000000-0000-0000-0000-000000000001" },
                    { "i0000000-0000-0000-0000-000000000002", "Sample studio", 1, "/images/properties/placeholder-2.jpg", true, "p0000000-0000-0000-0000-000000000002" },
                    { "i0000000-0000-0000-0000-000000000003", "Sample house", 1, "/images/properties/placeholder-3.jpg", true, "p0000000-0000-0000-0000-000000000003" },
                    { "i0000000-0000-0000-0000-000000000004", "Sample lot", 1, "/images/properties/placeholder-4.jpg", true, "p0000000-0000-0000-0000-000000000004" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PropertyImages",
                keyColumn: "Id",
                keyValue: "i0000000-0000-0000-0000-000000000001");

            migrationBuilder.DeleteData(
                table: "PropertyImages",
                keyColumn: "Id",
                keyValue: "i0000000-0000-0000-0000-000000000002");

            migrationBuilder.DeleteData(
                table: "PropertyImages",
                keyColumn: "Id",
                keyValue: "i0000000-0000-0000-0000-000000000003");

            migrationBuilder.DeleteData(
                table: "PropertyImages",
                keyColumn: "Id",
                keyValue: "i0000000-0000-0000-0000-000000000004");

            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: "p0000000-0000-0000-0000-000000000001");

            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: "p0000000-0000-0000-0000-000000000002");

            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: "p0000000-0000-0000-0000-000000000003");

            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: "p0000000-0000-0000-0000-000000000004");

            migrationBuilder.DeleteData(
                table: "Agents",
                keyColumn: "Id",
                keyValue: "a0000000-0000-0000-0000-000000000001");

            migrationBuilder.DeleteData(
                table: "Agents",
                keyColumn: "Id",
                keyValue: "a0000000-0000-0000-0000-000000000002");

            migrationBuilder.DeleteData(
                table: "Developers",
                keyColumn: "Id",
                keyValue: "d0000000-0000-0000-0000-000000000001");

            migrationBuilder.DeleteData(
                table: "Developers",
                keyColumn: "Id",
                keyValue: "d0000000-0000-0000-0000-000000000002");
        }
    }
}
