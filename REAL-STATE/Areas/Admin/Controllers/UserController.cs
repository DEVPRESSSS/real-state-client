using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using REAL_STATE.Models.VM;
using REAL_STATE.Services.Interfaces;
using REAL_STATE.Utilities;

namespace REAL_STATE.Areas.Admin.Controllers
{
    [Area(RoleSD.Admin)]
    [Authorize(Roles = RoleSD.Admin)]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(IUserService userService, RoleManager<IdentityRole> roleManager)
        {
            _userService = userService;
            _roleManager = roleManager;
        }

        private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsers(CurrentUserId);
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(string? id)
        {
            var vm = new UserVM();

            if (!string.IsNullOrEmpty(id))
            {
                vm = await _userService.GetUserForEdit(id);
                if (vm == null)
                    return NotFound();
            }

            LoadRoles(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(UserVM vm)
        {
            vm.IsEdit = !string.IsNullOrEmpty(vm.Id);

            if (!vm.IsEdit && string.IsNullOrWhiteSpace(vm.Password))
                ModelState.AddModelError(nameof(vm.Password), "Password is required for new users.");

            if (!ModelState.IsValid)
            {
                LoadRoles(vm);
                return View(vm);
            }

            try
            {
                if (!vm.IsEdit)
                {
                    await _userService.AddUser(vm);
                    TempData["SuccessMessage"] = "User added successfully.";
                }
                else
                {
                    await _userService.UpdateUser(vm);
                    TempData["SuccessMessage"] = "User updated successfully.";
                }
            }
            catch (Exception ex)
            {
                // Stay on the form so the admin doesn't lose what they typed
                ModelState.AddModelError(string.Empty, ex.Message);
                LoadRoles(vm);
                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLock(string id, bool locked)
        {
            try
            {
                await _userService.SetLock(id, locked, CurrentUserId);
                return Json(new
                {
                    success = true,
                    message = locked ? "User account locked." : "User account unlocked."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        private void LoadRoles(UserVM vm)
        {
            vm.Roles = _roleManager.Roles
                .OrderBy(r => r.Name)
                .Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
                .ToList();
        }
    }
}