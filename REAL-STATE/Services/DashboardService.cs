using Microsoft.EntityFrameworkCore;
using REAL_STATE.Data;
using REAL_STATE.Models.VM;
using REAL_STATE.Services.Interfaces;
using static REAL_STATE.Models.VM.DashboardVM;

namespace REAL_STATE.Services;

public sealed class DashboardService(ApplicationDbContext db) : IDashboardService
{
    public async Task<DashboardVM> GetAsync()
    {
        var vm = await db.Properties.AsNoTracking().GroupBy(p => 1).Select(g => new DashboardVM
        {
            TotalProperties = g.Count(), AvailableProperties = g.Count(p => p.Status == "Available"),
            FeaturedProperties = g.Count(p => p.IsFeatured), ForSale = g.Count(p => p.ListingType == "Sale"),
            ForRent = g.Count(p => p.ListingType == "Rent")
        }).SingleOrDefaultAsync() ?? new DashboardVM();
        var developers = await db.Developers.GroupBy(d => 1).Select(g => new { Total = g.Count(), Active = g.Count(d => d.IsActive) }).SingleOrDefaultAsync();
        vm.TotalDevelopers = developers?.Total ?? 0; vm.ActiveDevelopers = developers?.Active ?? 0;
        var agents = await db.Agents.GroupBy(a => 1).Select(g => new { Total = g.Count(), Active = g.Count(a => a.IsActive) }).SingleOrDefaultAsync();
        vm.TotalAgents = agents?.Total ?? 0; vm.ActiveAgents = agents?.Active ?? 0;
        var now = DateTimeOffset.UtcNow;
        var users = await db.Users.GroupBy(u => 1).Select(g => new { Total = g.Count(), Locked = g.Count(u => u.LockoutEnd.HasValue && u.LockoutEnd > now) }).SingleOrDefaultAsync();
        vm.TotalUsers = users?.Total ?? 0; vm.LockedUsers = users?.Locked ?? 0;
        vm.RecentProperties = await db.Properties.AsNoTracking().Include(p => p.Developer).Include(p => p.Agent)
            .OrderByDescending(p => p.CreatedAt).ThenBy(p => p.Id).Take(6).ToListAsync();
        vm.PropertiesByStatus = await db.Properties.GroupBy(p => p.Status == "" ? "Unknown" : p.Status)
            .Select(g => new StatusCount { Label = g.Key, Count = g.Count() }).OrderByDescending(x => x.Count).ToListAsync();
        vm.PropertiesByType = await db.Properties.GroupBy(p => p.PropertyType == "" ? "Unknown" : p.PropertyType)
            .Select(g => new StatusCount { Label = g.Key, Count = g.Count() }).OrderByDescending(x => x.Count).Take(5).ToListAsync();
        return vm;
    }
}
