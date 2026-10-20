using Microsoft.EntityFrameworkCore;
using REAL_STATE.Data;
using REAL_STATE.Models.DTO.PropertyDTO;
using REAL_STATE.Models.DTO.PropertyDTO.REAL_STATE.DTOs;
using REAL_STATE.Services.Interfaces;

namespace REAL_STATE.Services
{
    public class PropertyService:IPropertyService
    {
        private readonly ApplicationDbContext _context;
        public PropertyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<FilterPropertyDTO>> Filter(CriteriaDTO? criteriaDTO)
        {
            var query = _context.Properties
                .AsNoTracking()
                .Where(p => p.Status == "Available"); 

            if (criteriaDTO != null)
            {
                if (!string.IsNullOrWhiteSpace(criteriaDTO.ListingType))
                {
                    var listingType = criteriaDTO.ListingType.Trim();
                    query = query.Where(p => p.ListingType == listingType);
                }

                if (!string.IsNullOrWhiteSpace(criteriaDTO.City))
                {
                    var city = criteriaDTO.City.Trim().ToLower();
                    query = query.Where(p => p.City.ToLower().Contains(city));
                }

                if (!string.IsNullOrWhiteSpace(criteriaDTO.PropertyType))
                {
                    var type = criteriaDTO.PropertyType.Trim().ToLower();
                    query = query.Where(p => p.PropertyType.ToLower().Contains(type));
                }

                if (criteriaDTO.MinPrice.HasValue)
                    query = query.Where(p => p.Price >= criteriaDTO.MinPrice.Value);

                if (criteriaDTO.MaxPrice.HasValue)
                    query = query.Where(p => p.Price <= criteriaDTO.MaxPrice.Value);

                if (criteriaDTO.MinFloorArea.HasValue)
                    query = query.Where(p => p.FloorAreaSqFt >= criteriaDTO.MinFloorArea.Value);
            }

            return await query
                .OrderByDescending(p => p.IsFeatured)
                .ThenByDescending(p => p.CreatedAt)
                .Select(p => new FilterPropertyDTO
                {
                    Id = p.Id,
                    Slug = p.Slug,
                    Title = p.Title,
                    PropertyType = p.PropertyType,
                    ListingType = p.ListingType,
                    Price = p.Price,
                    FloorAreaSqFt = p.FloorAreaSqFt,
                    Bedrooms = p.Bedrooms,
                    Bathrooms = p.Bathrooms,
                    Address = p.Address,
                    City = p.City,
                    Province = p.Province,
                    IsFeatured = p.IsFeatured,
                    DeveloperName = p.Developer.Name,
                    PrimaryImageUrl = p.Images
                        .OrderByDescending(i => i.IsPrimary)
                        .ThenBy(i => i.DisplayOrder)
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault(),
                    PrimaryImageAlt = p.Images
                        .OrderByDescending(i => i.IsPrimary)
                        .ThenBy(i => i.DisplayOrder)
                        .Select(i => i.AltText)
                        .FirstOrDefault()
                })
                .ToListAsync();
        }

       
    }
}
