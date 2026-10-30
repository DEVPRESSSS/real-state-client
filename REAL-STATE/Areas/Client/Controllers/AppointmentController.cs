using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAL_STATE.Models.VM;
using REAL_STATE.Services;
using REAL_STATE.Services.Interfaces;
using REAL_STATE.Utilities;

namespace REAL_STATE.Controllers
{
    [Area("Client")]
    //[Authorize(Roles = RoleSD.User)]
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointments;

        public AppointmentController(IAppointmentService appointments)
        {
            _appointments = appointments;
        }

        [HttpGet]
        public async Task<IActionResult> Create(string? propertyId)
        {
            if (string.IsNullOrWhiteSpace(propertyId)) return NotFound();

            var vm = await _appointments.GetFormAsync(propertyId, User);
            if (vm == null) return NotFound();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ScheduleAppointmentVM vm)
        {
            var result = await _appointments.CreateAsync(vm, User, ModelState.IsValid);

            if (result.PropertyNotFound) return NotFound();

            foreach (var (key, message) in result.Errors)
                ModelState.AddModelError(key, message);

            if (!result.Success) return View(vm); 

            var appointment = result.Appointment!;
            TempData["Viewing.Property"] = result.PropertyTitle;
            TempData["Viewing.When"] = appointment.ScheduledAt.ToString("dddd, MMMM d, yyyy 'at' h:mm tt");
            TempData["Viewing.Email"] = appointment.ContactEmail;
            TempData["Viewing.PropertyId"] = appointment.PropertyId;

            return RedirectToAction(nameof(Confirmation));
        }

        [HttpGet]
        public IActionResult Confirmation()
        {
            if (TempData["Viewing.Property"] is not string title)
                return RedirectToAction("Index", "Properties");

            ViewData["Property"] = title;
            ViewData["When"] = TempData["Viewing.When"];
            ViewData["Email"] = TempData["Viewing.Email"];
            ViewData["PropertyId"] = TempData["Viewing.PropertyId"];
            return View();
        }
    }
}