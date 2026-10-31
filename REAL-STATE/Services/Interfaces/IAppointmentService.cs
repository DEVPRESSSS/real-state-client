using REAL_STATE.Models.DTO.AppointmentDTO;
using REAL_STATE.Models.VM;
using System.Security.Claims;

namespace REAL_STATE.Services.Interfaces
{
    public interface IAppointmentService
    {
        /// <summary>
        /// Builds the form for a property. Pre-fills contact details for signed-in users.
        /// Returns null when the property does not exist.
        /// </summary>
        Task<ScheduleAppointmentVM?> GetFormAsync(string propertyId, ClaimsPrincipal user);

        /// <summary>
        /// Re-fills the display-only property fields on an existing view model.
        /// Returns false when the property does not exist.
        /// </summary>
        Task<bool> FillPropertySummaryAsync(ScheduleAppointmentVM vm);

        /// <summary>
        /// Applies business rules and saves a Pending appointment.
        /// Nothing is saved unless inputIsValid is true and no business rule fails.
        /// </summary>
        Task<CreateAppointmentResultDTO> CreateAsync(
            ScheduleAppointmentVM vm, ClaimsPrincipal user, bool inputIsValid);

        Task<PagedResult<AppointmentListVM>> GetAppointmentsAsync(
           int page,
           int pageSize
       );
    }
}
