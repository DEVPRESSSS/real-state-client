using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using REAL_STATE.Models;
using REAL_STATE.Services.Interfaces;
using REAL_STATE.Utilities;

namespace REAL_STATE.Areas.AgentAndBroker.Controllers
{
    [Area("AgentAndBroker")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    [Authorize(Policy = "ActiveAgent")]
    public class RefferalController : Controller
    {
        private readonly IRefferalService _referralService;
        private readonly UserManager<ApplicationUser> _userManager;

        public RefferalController(IRefferalService referralService, UserManager<ApplicationUser> userManager)
        {
            _referralService = referralService;
            _userManager = userManager;
        }

        private string BuildLink(string code) =>
            Url.Page("/Account/Register", pageHandler: null,
                     values: new { area = "Identity", @ref = code },
                     protocol: Request.Scheme)!;

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;
            var vm = await _referralService.GetAllAndLatestReferralService(userId, BuildLink);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Generate()
        {
            var userId = _userManager.GetUserId(User)!;
            await _referralService.GenerateAsyncService(userId);

            TempData["ReferralMessage"] = "Your referral link is ready.";
            return RedirectToAction(nameof(Index));
        }
    }
}
