using Microsoft.EntityFrameworkCore;
using REAL_STATE.Data;
using REAL_STATE.Models;
using REAL_STATE.Models.VM;
using REAL_STATE.Repositories.IRepositories;

namespace REAL_STATE.Repositories
{
    public class ViewingAppointmentRepository: Repository<ViewingAppointment>, IAppointmentRepository
    {
        public ViewingAppointmentRepository(ApplicationDbContext db)
          : base(db)
        {
        }
        public async Task<PagedResult<AppointmentListVM>> GetPagedAsync(
            int page,
            int pageSize)
        {
            var query = _db.ViewingAppointments
                .AsNoTracking();

            var totalItems = await query.CountAsync();

            var appointments = await query
                .OrderByDescending(a => a.CreatedAt)
                .ThenBy(a => a.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AppointmentListVM
                {
                    Id = a.Id,
                    Property = a.Property.Title,
                    ContactName = a.ContactName,
                    Email = a.ContactEmail,
                    Phone = a.ContactPhone,
                    Agent = a.Agent != null
                        ? a.Agent.FullName
                        : "Unassigned",
                    ScheduledAt = a.ScheduledAt,
                    Status = a.Status,
                    Notes = a.Notes,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();

            return new PagedResult<AppointmentListVM>
            {
                Items = appointments,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }
    }
}
