using REAL_STATE.Models.VM;
namespace REAL_STATE.Services.Interfaces;
public interface IDashboardService { Task<DashboardVM> GetAsync(); }
