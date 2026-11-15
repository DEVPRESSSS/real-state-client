using Microsoft.EntityFrameworkCore;
using REAL_STATE.Data;
using REAL_STATE.Models;
using REAL_STATE.Models.VM;
using REAL_STATE.Repositories.IRepositories;
using REAL_STATE.Utilities;

namespace REAL_STATE.Repositories;

public sealed class ViewingAppointmentRepository(ApplicationDbContext db)
    : Repository<ViewingAppointment>(db), IAppointmentRepository
{
    public async Task<bool> ApproveAsync(string appointmentId, string userId, bool isAdmin)
    {
        var changed = await _db.ViewingAppointments
            .Where(a => a.Id == appointmentId && a.Status == "Pending"
                && (isAdmin || (a.Agent != null && a.Agent.UserId == userId && a.Agent.IsActive)))
            .ExecuteUpdateAsync(update => update.SetProperty(a => a.Status, "Approved"));
        return changed == 1;
    }

    public Task<PagedResult<AppointmentListVM>> GetPagedForAgentAsync(int page, int pageSize, string userId, string? q = null)
        => GetPageAsync(_db.ViewingAppointments.Where(a => a.Agent != null
            && a.Agent.UserId == userId && a.Agent.IsActive), page, pageSize, true, q);

    public Task<PagedResult<AppointmentListVM>> GetPagedAsync(int page, int pageSize, string? q = null)
        => GetPageAsync(_db.ViewingAppointments, page, pageSize, false, q);

    private static Task<PagedResult<AppointmentListVM>> GetPageAsync(
        IQueryable<ViewingAppointment> query, int page, int pageSize, bool forAgent, string? q)
    {
        var term = q?.Trim();
        if (!string.IsNullOrEmpty(term))
        {
            query = query.Where(a => a.Property.Title.Contains(term) || a.ContactName.Contains(term)
                || a.ContactEmail.Contains(term) || (a.ContactPhone != null && a.ContactPhone.Contains(term))
                || a.Status.Contains(term) || (a.Agent != null && a.Agent.FullName.Contains(term)));
        }

        return query.AsNoTracking().OrderByDescending(a => a.CreatedAt).ThenBy(a => a.Id)
            .Select(a => new AppointmentListVM
            {
                Id = a.Id, Property = a.Property.Title, ContactName = a.ContactName,
                Email = a.ContactEmail, Phone = a.ContactPhone,
                Agent = a.Agent == null ? "Unassigned" : forAgent ? "You" : a.Agent.FullName,
                IsPropertyReferral = a.ReferralAgentId != null,
                ScheduledAt = a.ScheduledAt, Status = a.Status, Notes = a.Notes, CreatedAt = a.CreatedAt
            }).ToPageAsync(page, pageSize);
    }
}
