using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using REAL_STATE.Data;
using REAL_STATE.Models.DTO.PropertyDTO;
using REAL_STATE.Models.VM;
using REAL_STATE.Repositories.IRepositories;
using REAL_STATE.Services.Interfaces;
using REAL_STATE.Utilities;

namespace REAL_STATE.Areas.Admin.Controllers
{
    [Area(RoleSD.Admin)]
    [Authorize(Roles = RoleSD.Admin)]
    public class PropertyController : Controller
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IDeveloperRepository _developerRepository;
        private readonly IAgentRepository _agentRepository;
        private readonly IPropertyService _propertyService;
        public PropertyController(
            IPropertyRepository propertyRepository, 
            IDeveloperRepository developerRepository,
            IAgentRepository agentRepository,
            IPropertyService propertyService)
        {
            _propertyRepository = propertyRepository;
            _developerRepository = developerRepository;
            _agentRepository = agentRepository;
            _propertyService = propertyService;
        }

        private async Task LoadListsAsync(PropertyVM vm)
        {
            var developers = await _developerRepository.GetAllAsync();
            var agents = await _agentRepository.GetAllAsync();

            vm.Developers = developers.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            }).ToList();

            vm.Agents = agents.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.FullName
            }).ToList();
        }

        [HttpGet]
        public async Task<IActionResult> Upsert()
        {
            var vm = new PropertyVM { Property = new() };
            await LoadListsAsync(vm);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Upsert(PropertyVM vm)
        {
            if (!ModelState.IsValid)
            {
                await LoadListsAsync(vm);
                return View(vm);
            }

            try
            {
                await _propertyService.AddProperty(vm);
                TempData["SuccessMessage"] = "Added property successfully";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Index()
        {
            var properties = await _propertyRepository.GetAllAsync(includeProperties: "Developer,Agent");

            return View(properties);
        }

    }
}
