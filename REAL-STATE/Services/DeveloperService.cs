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

        public DeveloperService(
            IDeveloperRepository developerRepository

            )
        {
            _developerRepository = developerRepository;
        }

        public async Task AddDeveloper(Developers developer)
        {
            await _developerRepository.AddAsync(developer);
            await _developerRepository.Save();
        }

        public async Task<List<DeveloperCardVM>> GetActiveDevelopers()
        {
            var developers = await _developerRepository.GetAllAsync(
                x => x.IsActive,
                includeProperties: "Properties");

            return developers
                .OrderBy(x => x.Name)
                .Select(d => new DeveloperCardVM
                {
                    Id = d.Id,
                    Name = d.Name,
                    LogoUrl = d.LogoUrl,
                    Description = d.Description,
                    WebsiteUrl = d.WebsiteUrl,
                    AvailableListings = d.Properties.Count(p => p.Status == "Available")
                })
                .ToList();
        }

        public async Task UpdateDeveloper(Developers developer)
        {
            var developerObj = await _developerRepository.GetAsync(x => x.Id == developer.Id);
            if (developerObj is null)
                throw new GlobalExceptionHandler("Developer not found!!");

            var slugTaken = (await _developerRepository.GetAllAsync(x => x.Slug == developer.Slug && x.Id != developer.Id)).Any();
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
