using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using REAL_STATE.Models;
using REAL_STATE.Models.VM;
using REAL_STATE.Repositories.IRepositories;
using REAL_STATE.Services.Interfaces;
using REAL_STATE.Utilities;

namespace REAL_STATE.Areas.Admin.Controllers
{
    [Area(RoleSD.Admin)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    [Authorize(Roles = RoleSD.Admin)]
    public class PropertyController : Controller
    {
        private readonly REAL_STATE.Utilities.SafeErrorMessages _errors;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IDeveloperRepository _developerRepository;
        private readonly IAgentRepository _agentRepository;
        private readonly IPropertyService _propertyService;

        public PropertyController(
            IPropertyRepository propertyRepository,
            IDeveloperRepository developerRepository,
            IAgentRepository agentRepository,
            IPropertyService propertyService,
            SafeErrorMessages errors)
        {
            _errors = errors;
            _propertyRepository = propertyRepository;
            _developerRepository = developerRepository;
            _agentRepository = agentRepository;
            _propertyService = propertyService;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 20, string? q = null)
        {
            q = q?.Trim();
            var result = await _propertyRepository.GetPageAsync(
                query => query.OrderBy(x => x.Title).ThenBy(x => x.Id), page, pageSize,
                x => q == null || q == "" || x.Title.Contains(q) || x.City.Contains(q) || x.PropertyType.Contains(q), includeProperties: "Developer,Agent");
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(string? id)
        {
            var vm = new PropertyVM();

            if (!string.IsNullOrEmpty(id))
            {
                var property = await _propertyRepository.GetAsync(x => x.Id == id);
                if (property == null)
                    return NotFound();

                vm.Property = PropertyInput.FromEntity(property);
                vm.IsEdit = true;
            }

            await LoadListsAsync(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(PropertyVM vm)
        {
            if (!ModelState.IsValid)
            {
                var exists = await _propertyRepository.GetAsync(x => x.Id == vm.Property.Id);
                vm.IsEdit = exists != null;
                await LoadListsAsync(vm);
                return View(vm);
            }

            if (string.IsNullOrWhiteSpace(vm.Property.AgentId))
                vm.Property.AgentId = null;

            try
            {
                var existing = await _propertyRepository.GetAsync(x => x.Id == vm.Property.Id);

                if (existing == null)
                {
                    await _propertyService.AddProperty(vm);
                    TempData["SuccessMessage"] = "Property added successfully.";
                }
                else
                {
                    await _propertyService.UpdateProperty(vm);
                    TempData["SuccessMessage"] = "Property updated successfully.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = _errors.For(ex);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var property = await _propertyRepository.GetAsync(x => x.Id == id);
            if (property == null)
                return NotFound(new { success = false, message = "Property not found." });

            try
            {
                 _propertyRepository.Remove(property);
                await _propertyRepository.Save();
                return Json(new { success = true, message = "Property deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = _errors.For(ex) });
            }
        }

        private async Task LoadListsAsync(PropertyVM vm)
        {
            var developers = await _developerRepository.GetAllAsync(x => x.IsActive);
            var agents = await _agentRepository.GetAllAsync(x => x.IsActive);

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
    }
}
