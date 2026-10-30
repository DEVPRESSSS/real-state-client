using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using REAL_STATE.Data;
using REAL_STATE.Models;
using REAL_STATE.Models.DTO.AppointmentDTO;
using REAL_STATE.Models.VM;
using REAL_STATE.Services.Interfaces;
using System.Security.Claims;

namespace REAL_STATE.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentService(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<ScheduleAppointmentVM?> GetFormAsync(string propertyId, ClaimsPrincipal user)
        {
            if (string.IsNullOrWhiteSpace(propertyId)) return null;

            var property = await LoadPropertyAsync(propertyId);
            if (property == null) return null;

            var vm = new ScheduleAppointmentVM { PropertyId = property.Id };
            FillPropertySummary(vm, property);

            if (user.Identity?.IsAuthenticated == true)
            {
                var appUser = await _userManager.GetUserAsync(user);
                if (appUser != null)
                {
                    vm.ContactName = appUser.FullName;
                    vm.ContactEmail = appUser.Email ?? string.Empty;
                    vm.ContactPhone = appUser.PhoneNumber;
                }
            }

            return vm;
        }

        public async Task<bool> FillPropertySummaryAsync(ScheduleAppointmentVM vm)
        {
            var property = await LoadPropertyAsync(vm.PropertyId);
            if (property == null) return false;

            FillPropertySummary(vm, property);
            return true;
        }

        public async Task<CreateAppointmentResultDTO> CreateAsync(
            ScheduleAppointmentVM vm, ClaimsPrincipal user, bool inputIsValid)
        {
            var property = await LoadPropertyAsync(vm.PropertyId);
            if (property == null)
                return new CreateAppointmentResultDTO { PropertyNotFound = true };

            FillPropertySummary(vm, property);

            var errors = new List<(string Key, string Message)>();

            if (!string.Equals(property.Status, "Available", StringComparison.OrdinalIgnoreCase))
            {
                errors.Add((string.Empty, "This property is no longer available for viewing requests."));
            }

            DateTime scheduledAt = default;
            if (vm.PreferredDate.HasValue && vm.PreferredTime.HasValue)
            {
                scheduledAt = vm.PreferredDate.Value.Date + vm.PreferredTime.Value.ToTimeSpan();
                if (scheduledAt <= DateTime.Now)
                {
                    errors.Add((nameof(vm.PreferredDate), "Please choose a date and time in the future."));
                }
            }

            if (!inputIsValid || errors.Count > 0)
            {
                return new CreateAppointmentResultDTO
                {
                    Errors = errors,
                    PropertyTitle = property.Title
                };
            }

            var appointment = new ViewingAppointment
            {
                PropertyId = property.Id,
                AgentId = property.AgentId,
                UserId = user.Identity?.IsAuthenticated == true ? _userManager.GetUserId(user) : null,
                ContactName = vm.ContactName.Trim(),
                ContactEmail = vm.ContactEmail.Trim(),
                ContactPhone = string.IsNullOrWhiteSpace(vm.ContactPhone) ? null : vm.ContactPhone.Trim(),
                ScheduledAt = scheduledAt,
                Notes = string.IsNullOrWhiteSpace(vm.Notes) ? null : vm.Notes.Trim(),
                Status = "Pending"
            };

            _db.ViewingAppointments.Add(appointment);
            await _db.SaveChangesAsync();

            return new CreateAppointmentResultDTO
            {
                Appointment = appointment,
                PropertyTitle = property.Title
            };
        }

        // ---------- helpers ----------

        private Task<Property?> LoadPropertyAsync(string propertyId) =>
            _db.Properties
               .AsNoTracking()
               .Include(p => p.Images)
               .Include(p => p.Agent)
               .FirstOrDefaultAsync(p => p.Id == propertyId);

        private static void FillPropertySummary(ScheduleAppointmentVM vm, Property p)
        {
            var image = p.Images.FirstOrDefault(i => i.IsPrimary)
                        ?? p.Images.OrderBy(i => i.DisplayOrder).FirstOrDefault();

            vm.PropertyTitle = p.Title;
            vm.PropertyLocation = string.Join(", ",
                new[] { p.Address, p.City, p.Province }.Where(s => !string.IsNullOrWhiteSpace(s)));
            vm.Price = p.Price;
            vm.ListingType = p.ListingType;
            vm.PrimaryImageUrl = image?.ImageUrl;
            vm.PrimaryImageAlt = image?.AltText ?? p.Title;
            vm.AgentName = p.Agent?.FullName;
        }
    }
}