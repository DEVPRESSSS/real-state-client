using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using REAL_STATE.Models;
using REAL_STATE.Models.VM;
using REAL_STATE.Repositories.IRepositories;
using REAL_STATE.Services;
using REAL_STATE.Services.Interfaces;
using REAL_STATE.Utilities;

namespace REAL_STATE.Areas.Admin.Controllers
{
    [Area(RoleSD.Admin)]
    [Authorize(Roles = RoleSD.Admin)]
    public class DeveloperController : Controller
    {
        private readonly IDeveloperRepository _developerRepository;
        private readonly IDeveloperService _developerService;

        public DeveloperController(
            IDeveloperRepository developerRepository,
            IDeveloperService developerService)
        {
            _developerRepository = developerRepository;
            _developerService = developerService;
        }

        public async Task<IActionResult> Index()
        {
            var developers = await _developerRepository.GetAllAsync();
            return View(developers);
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(string? id)
        {

            if (!string.IsNullOrEmpty(id))
            {
                var developer = await _developerRepository.GetAsync(x => x.Id == id);
                if (developer == null)
                    return NotFound();
                return View(developer);

            }

            return View(new Developers());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Developers developer)
        {
            if (!ModelState.IsValid)
            {
                var developerExist = await _developerRepository.GetAsync(x => x.Id == developer.Id);
                return View(developerExist);
            }

            try
            {
                var existing = await _developerRepository.GetAsync(x => x.Id == developer.Id);

                if (existing == null)
                {
                    await _developerService.AddDeveloper(developer);
                    TempData["SuccessMessage"] = "Developer added successfully.";
                }
                else
                {
                    await _developerService.UpdateDeveloper(developer);
                    TempData["SuccessMessage"] = "Developer updated successfully.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var developer = await _developerRepository.GetAsync(x => x.Id == id);
            if (developer == null)
                return NotFound(new { success = false, message = "Developer not found." });

            try
            {
                _developerRepository.Remove(developer);
                await _developerRepository.Save();
                return Json(new { success = true, message = "Developer deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

    }
}