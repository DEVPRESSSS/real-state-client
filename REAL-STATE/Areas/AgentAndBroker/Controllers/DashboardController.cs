using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAL_STATE.Utilities;

namespace REAL_STATE.Areas.AgentAndBroker.Controllers
{
    [Area("AgentAndBroker")]
    [Authorize(Roles = RoleSD.Agent)]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
