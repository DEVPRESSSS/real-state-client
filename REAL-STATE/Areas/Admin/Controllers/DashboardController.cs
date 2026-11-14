using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAL_STATE.Services.Interfaces;
using REAL_STATE.Utilities;
namespace REAL_STATE.Areas.Admin.Controllers;
[Area(RoleSD.Admin)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
[Authorize(Roles = RoleSD.Admin)]
public sealed class DashboardController(IDashboardService dashboard) : Controller
{
    public async Task<IActionResult> Index() => View(await dashboard.GetAsync());
}
