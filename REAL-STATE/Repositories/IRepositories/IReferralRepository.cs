using REAL_STATE.Models;
using REAL_STATE.Models.VM;

namespace REAL_STATE.Repositories.IRepositories
{
    public interface IReferralRepository:IRepository<Referral>
    {
        Task<ReferralIndexVM> GetAllAndLatestReferralService(string userId, Func<string, string> buildLink);
        Task<string> GenerateAsync(string userId);
        Task<bool> RedeemAsync(string code, string newUserId);
    }
}
