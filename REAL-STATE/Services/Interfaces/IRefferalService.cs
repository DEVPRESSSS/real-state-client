using REAL_STATE.Models.VM;

namespace REAL_STATE.Services.Interfaces
{
    public interface IRefferalService
    {
        Task<ReferralIndexVM> GetAllAndLatestReferralService(string userId, Func<string, string> buildLink);
        Task<string> GenerateAsyncService(string userId);
    }
}
