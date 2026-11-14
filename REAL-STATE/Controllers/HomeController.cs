using REAL_STATE.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAL_STATE.Models;
using REAL_STATE.Models.DTO.PropertyDTO;
using REAL_STATE.Models.VM;
using REAL_STATE.Services;
using REAL_STATE.Services.Interfaces;
using System.Diagnostics;

namespace REAL_STATE.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly REAL_STATE.Utilities.SafeErrorMessages _errors;
        private readonly IPropertyService _propertyService;
        private readonly IDeveloperService _developerService;
        private readonly IAgentService _agentService;
        private const int PageSize = 10;

        public HomeController(IPropertyService propertyService,

            IAgentService agentService,
            IDeveloperService developerService,
            REAL_STATE.Utilities.SafeErrorMessages errors)
        {
            _errors = errors;
            _propertyService = propertyService;
            _agentService = agentService;
            _developerService= developerService;
        }
        public async Task<IActionResult> Hero()
        {
            return View();
        }

        public async Task<IActionResult> Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Index([FromQuery] CriteriaDTO criteria)
        {
            var properties = await _propertyService.Filter(criteria);
            return View(properties);
        }
        public async Task<IActionResult> Agents(int page = 1)
        {
            var vm = await _agentService.GetAgentsPage(page, PageSize);
            return View(vm);
        }

        [HttpGet]
        public IActionResult Register() => View(new AgentRegisterVM());

        [HttpPost]
        [Microsoft.AspNetCore.RateLimiting.EnableRateLimiting(REAL_STATE.Utilities.RateLimitConfiguration.PublicWrites)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AgentRegisterVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                await _agentService.RegisterAgent(vm);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, _errors.For(ex));
                return View(vm);
            }

            TempData["SuccessMessage"] = "Registration received. Check your email to confirm your account. An admin will review your profile.";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var vm = await _propertyService.GetDetails(id);
            if (vm == null)
                return NotFound();

            return View(vm);
        }

        public async Task<IActionResult> Developers(int page = 1)
        {
            var developers = await _developerService.GetActiveDevelopers(page);
            return View(developers);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
