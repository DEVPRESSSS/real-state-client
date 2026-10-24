using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAL_STATE.Models.VM;
using REAL_STATE.Repositories.IRepositories;
using REAL_STATE.Utilities;
using static REAL_STATE.Models.VM.DashboardVM;

namespace REAL_STATE.Areas.Admin.Controllers
{
    [Area(RoleSD.Admin)]
    [Authorize(Roles = RoleSD.Admin)]
    public class DashboardController : Controller
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IDeveloperRepository _developerRepository;
        private readonly IAgentRepository _agentRepository;
        private readonly IUserRepository _userRepository;

        public DashboardController(
            IPropertyRepository propertyRepository,
            IDeveloperRepository developerRepository,
            IAgentRepository agentRepository,
            IUserRepository userRepository)
        {
            _propertyRepository = propertyRepository;
            _developerRepository = developerRepository;
            _agentRepository = agentRepository;
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Index()
        {
            var properties = (await _propertyRepository.GetAllAsync(includeProperties: "Developer,Agent")).ToList();
            var developers = (await _developerRepository.GetAllAsync()).ToList();
            var agents = (await _agentRepository.GetAllAsync()).ToList();
            var users = (await _userRepository.GetAllAsync()).ToList();

            var now = DateTimeOffset.UtcNow;

            var vm = new DashboardVM
            {
                TotalProperties = properties.Count,
                AvailableProperties = properties.Count(x => x.Status == "Available"),
                FeaturedProperties = properties.Count(x => x.IsFeatured),
                ForSale = properties.Count(x => x.ListingType == "Sale"),
                ForRent = properties.Count(x => x.ListingType == "Rent"),

                TotalDevelopers = developers.Count,
                ActiveDevelopers = developers.Count(x => x.IsActive),

                TotalAgents = agents.Count,
                ActiveAgents = agents.Count(x => x.IsActive),

                TotalUsers = users.Count,
                LockedUsers = users.Count(x => x.LockoutEnd.HasValue && x.LockoutEnd.Value > now),

                RecentProperties = properties
                    .OrderByDescending(x => x.CreatedAt)
                    .Take(6)
                    .ToList(),

                PropertiesByStatus = properties
                    .GroupBy(x => string.IsNullOrWhiteSpace(x.Status) ? "Unknown" : x.Status)
                    .Select(g => new StatusCount { Label = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .ToList(),

                PropertiesByType = properties
                    .GroupBy(x => string.IsNullOrWhiteSpace(x.PropertyType) ? "Unknown" : x.PropertyType)
                    .Select(g => new StatusCount { Label = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(5)
                    .ToList()
            };

            return View(vm);
        }
    }
}