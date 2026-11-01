using REAL_STATE.Models;
using REAL_STATE.Models.VM;

namespace REAL_STATE.Repositories.IRepositories
{
    public interface IAppointmentRepository: IRepository<ViewingAppointment>
    {
        Task<PagedResult<AppointmentListVM>> GetPagedAsync(
            int page,
            int pageSize
        );

        Task<PagedResult<AppointmentListVM>> GetPagedAppointentForAgentAsync(
             int page,
             int pageSize,
             string userId
         );

    }
}
