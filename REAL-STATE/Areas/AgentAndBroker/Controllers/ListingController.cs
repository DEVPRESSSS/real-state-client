using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using REAL_STATE.Models;
using REAL_STATE.Models.VM;
using REAL_STATE.Repositories.IRepositories;
using REAL_STATE.Services.Interfaces;
using REAL_STATE.Utilities;

namespace REAL_STATE.Areas.AgentAndBroker.Controllers
{
    [Area("AgentAndBroker")]
    [Authorize(Roles = RoleSD.Agent)]
    public class ListingController : Controller
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IDeveloperRepository _developerRepository;
        private readonly IAgentRepository _agentRepository;
        private readonly IPropertyService _propertyService;

        public ListingController(
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

        private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        private Task<Agent?> GetCurrentAgentAsync() =>
            _agentRepository.GetAsync(x => x.UserId == CurrentUserId);

        public async Task<IActionResult> Index()
        {
            var agent = await GetCurrentAgentAsync();
            if (agent == null) return Forbid();

            var properties = await _propertyRepository.GetAllAsync(
                x => x.AgentId == agent.Id,
                includeProperties: "Developer");

            ViewBag.IsAgentActive = agent.IsActive;
            return View(properties);
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(string? id)
        {
            var agent = await GetCurrentAgentAsync();
            if (agent == null) return Forbid();

            var vm = new PropertyVM();

            if (!string.IsNullOrEmpty(id))
            {
                var property = await _propertyRepository.GetAsync(x => x.Id == id && x.AgentId == agent.Id);
                if (property == null)
                    return NotFound();   

                vm.Property = property;
                vm.IsEdit = true;
            }

            await LoadDevelopersAsync(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(PropertyVM vm)
        {
            var agent = await GetCurrentAgentAsync();
            if (agent == null) return Forbid();

            if (!agent.IsActive)
            {
                TempData["ErrorMessage"] = "Your account is still pending approval.";
                return RedirectToAction(nameof(Index));
            }

            var existing = await _propertyRepository.GetAsync(x => x.Id == vm.Property.Id);

            if (existing != null && existing.AgentId != agent.Id)
                return Forbid();   

            vm.IsEdit = existing != null;
            vm.Property.AgentId = agent.Id;
            vm.Property.IsFeatured = existing?.IsFeatured ?? false;   
            ModelState.Remove("Property.AgentId");
            ModelState.Remove("Property.IsFeatured");

            if (!ModelState.IsValid)
            {
                await LoadDevelopersAsync(vm);
                return View(vm);
            }

            try
            {
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
                ModelState.AddModelError(string.Empty, ex.Message);
                await LoadDevelopersAsync(vm);
                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var agent = await GetCurrentAgentAsync();
            if (agent == null)
                return StatusCode(403, new { success = false, message = "No agent profile is linked to your account." });

            var property = await _propertyRepository.GetAsync(x => x.Id == id && x.AgentId == agent.Id);
            if (property == null)
                return NotFound(new { success = false, message = "Listing not found." });

            try
            {
                _propertyRepository.Remove(property);  
                await _propertyRepository.Save();
                return Json(new { success = true, message = "Listing deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        private async Task LoadDevelopersAsync(PropertyVM vm)
        {
            var developers = await _developerRepository.GetAllAsync(x => x.IsActive);

            vm.Developers = developers.Select(x => new SelectListItem
            {
                Value = x.Id,
                Text = x.Name
            }).ToList();
        }
    }
}