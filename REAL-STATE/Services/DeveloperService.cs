using Microsoft.EntityFrameworkCore;
using REAL_STATE.Data;
using REAL_STATE.Models;
using REAL_STATE.Models.DTO.PropertyDTO;
using REAL_STATE.Models.DTO.PropertyDTO.REAL_STATE.DTOs;
using REAL_STATE.Models.VM;
using REAL_STATE.Repositories.IRepositories;
using REAL_STATE.Services.Interfaces;
using REAL_STATE.Utilities;

namespace REAL_STATE.Services
{
    public class DeveloperService:IDeveloperService
    {
        private readonly IDeveloperRepository _developerRepository;
        private readonly ApplicationDbContext _db;

        public DeveloperService(
            IDeveloperRepository developerRepository, ApplicationDbContext db

            )
        {
            _developerRepository = developerRepository;
            _db = db;
        }

        public async Task AddDeveloper(Developers developer)
        {
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(developer,
                new System.ComponentModel.DataAnnotations.ValidationContext(developer), true);
            await _developerRepository.AddAsync(new Developers { Name = developer.Name, Slug = developer.Slug,
                Description = developer.Description, WebsiteUrl = developer.WebsiteUrl,
                LogoUrl = developer.LogoUrl, IsActive = developer.IsActive });
            await _developerRepository.Save();
        }

        public async Task<PagedResult<DeveloperCardVM>> GetActiveDevelopers(int page = 1, int pageSize = 12)
        {
            return await _db.Developers.AsNoTracking().Where(d => d.IsActive)
                .OrderBy(d => d.Name).ThenBy(d => d.Id)
                .Select(d => new DeveloperCardVM
                {
                    Id = d.Id,
                    Name = d.Name,
                    LogoUrl = d.LogoUrl,
                    Description = d.Description,
                    WebsiteUrl = d.WebsiteUrl,
                    AvailableListings = d.Properties.Count(p => p.Status == "Available")
                })
                .ToPageAsync(page, pageSize);
        }

        public async Task UpdateDeveloper(Developers developer)
        {
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(developer,
                new System.ComponentModel.DataAnnotations.ValidationContext(developer), true);
            var developerObj = await _developerRepository.GetAsync(x => x.Id == developer.Id);
            if (developerObj is null)
                throw new GlobalExceptionHandler("Developer not found!!");

            var slugTaken = await _developerRepository.GetAsync(x => x.Slug == developer.Slug && x.Id != developer.Id) != null;
            if (slugTaken)
                throw new GlobalExceptionHandler("Slug is already in use.");

            developerObj.Name = developer.Name;
            developerObj.Slug = developer.Slug;
            developerObj.Description = developer.Description;
            developerObj.WebsiteUrl = developer.WebsiteUrl;
            developerObj.IsActive = developer.IsActive;

            if (!string.IsNullOrWhiteSpace(developer.LogoUrl))
                developerObj.LogoUrl = developer.LogoUrl;

            await _developerRepository.UpdateAsync(developerObj);
            await _developerRepository.Save();
        }


    }
}
