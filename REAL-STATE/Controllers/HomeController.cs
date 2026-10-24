using Microsoft.AspNetCore.Mvc;
using REAL_STATE.Models;
using REAL_STATE.Models.DTO.PropertyDTO;
using REAL_STATE.Models.VM;
using REAL_STATE.Services.Interfaces;
using System.Diagnostics;

namespace REAL_STATE.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly IAgentService _agentService;
        private const int PageSize = 10;  

        public HomeController(IPropertyService propertyService, IAgentService agentService)
        {
            _propertyService = propertyService;
            _agentService = agentService;
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
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }

            TempData["SuccessMessage"] = "Registration received. An admin will review and activate your profile.";
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
