using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAL_STATE.Repositories.IRepositories;
using REAL_STATE.Services.Interfaces;
using REAL_STATE.Utilities;
using System.Security.Claims;

namespace REAL_STATE.Areas.Admin.Controllers
{
    [Area(RoleSD.Admin)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    [Authorize(Roles = $"{RoleSD.Admin},{RoleSD.Agent}")]
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        public AppointmentController(IAppointmentService appointmentService)
        {
           _appointmentService = appointmentService;
        }
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            if (User.IsInRole(RoleSD.Admin))
            {
                var all = await _appointmentService.GetAppointmentsAsync(page, pageSize);
                return View(all);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Forbid();

            var mine = await _appointmentService.GetAppointmentsForAgentAsync( page, pageSize, userId);
            return View(mine);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(string appointmentId)
        {
            if (string.IsNullOrWhiteSpace(appointmentId))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid appointment ID."
                });
            }

            var result = await _appointmentService
                .ApproveAsync(appointmentId, User);

            if (!result.Success)
            {
                return BadRequest(new
                {
                    success = false,
                    message = result.Message
                });
            }

            return Json(new
            {
                success = true,
                message = result.Message
            });
        }
    }
}
