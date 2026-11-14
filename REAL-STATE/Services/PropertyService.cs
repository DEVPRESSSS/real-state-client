using Microsoft.EntityFrameworkCore;
using REAL_STATE.Data;
using REAL_STATE.Models.DTO.PropertyDTO;
using REAL_STATE.Models.DTO.PropertyDTO.REAL_STATE.DTOs;
using REAL_STATE.Models.VM;
using REAL_STATE.Repositories.IRepositories;
using REAL_STATE.Services.Interfaces;
using REAL_STATE.Utilities;

namespace REAL_STATE.Services
{
    public class PropertyService:IPropertyService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPropertyRepository _propertyRepository;

        public PropertyService(
            ApplicationDbContext context,
            IPropertyRepository propertyRepository

            )
        {
            _context = context;
            _propertyRepository = propertyRepository;
        }

        public async Task AddProperty(PropertyVM vm)
        {
            await ValidateReferencesAsync(vm.Property);
            var property = new REAL_STATE.Models.Property();
            vm.Property.ApplyTo(property);
            await _propertyRepository.AddAsync(property);
            await _propertyRepository.Save();
        }

        public async Task UpdateProperty(PropertyVM vm)
        {
            await ValidateReferencesAsync(vm.Property);
            var property = await _propertyRepository.GetAsync(x => x.Id == vm.Property.Id, tracked: true)
                ?? throw new GlobalExceptionHandler("Property not found.");
            vm.Property.ApplyTo(property);
            property.UpdatedAt = DateTime.UtcNow;
            await _propertyRepository.Save();
        }

        private async Task ValidateReferencesAsync(PropertyInput input)
        {
            var errors = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            if (!System.ComponentModel.DataAnnotations.Validator.TryValidateObject(input,
                new System.ComponentModel.DataAnnotations.ValidationContext(input), errors, true))
                throw new GlobalExceptionHandler(string.Join(" ", errors.Select(e => e.ErrorMessage)));
            if (!await _context.Developers.AnyAsync(d => d.Id == input.DeveloperId && d.IsActive))
                throw new GlobalExceptionHandler("Select an active developer.");
            if (!string.IsNullOrWhiteSpace(input.AgentId) &&
                !await _context.Agents.AnyAsync(a => a.Id == input.AgentId && a.IsActive))
                throw new GlobalExceptionHandler("Select an active agent.");
            if (await _context.Properties.AnyAsync(p => p.Slug == input.Slug && p.Id != input.Id))
                throw new GlobalExceptionHandler("Slug is already in use.");
        }

        public async Task<PagedResult<FilterPropertyDTO>> Filter(CriteriaDTO? criteriaDTO)
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
                    var city = criteriaDTO.City.Trim();
                    query = query.Where(p => p.City.StartsWith(city));
                }

                if (!string.IsNullOrWhiteSpace(criteriaDTO.PropertyType))
                {
                    var type = criteriaDTO.PropertyType.Trim();
                    query = query.Where(p => p.PropertyType == type);
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
                .ThenBy(p => p.Id)
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
                    DeveloperName = p.Developer != null ? p.Developer.Name : "",
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
                .ToPageAsync(criteriaDTO?.Page ?? 1, criteriaDTO?.PageSize ?? 12);
        }

        public async Task<PropertyDetailsVM?> GetDetails(string id)
        {
            var property = await _propertyRepository.GetAsync(x => x.Id == id,
                includeProperties: "Developer,Agent,Images");

            if (property == null)
                return null;

            return new PropertyDetailsVM
            {
                Property = property,
                Agent = property.Agent is { IsActive: true } ? property.Agent : null
            };
        }
    }
}
