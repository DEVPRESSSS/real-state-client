using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAL_STATE.Repositories.IRepositories;
using REAL_STATE.Services.Interfaces;
using REAL_STATE.Utilities;

namespace REAL_STATE.Areas.Admin.Controllers
{
    [Area(RoleSD.Admin)]
    [Authorize(Roles = RoleSD.Admin)]
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        public AppointmentController(IAppointmentService appointmentService)
        {
           _appointmentService = appointmentService;
        }
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var result = await _appointmentService.GetAppointmentsAsync(
                page,
                pageSize
            );

            return View(result);
        }
    }
}
