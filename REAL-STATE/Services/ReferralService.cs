using Microsoft.AspNetCore.Identity;
using REAL_STATE.Models;
using REAL_STATE.Models.VM;
using REAL_STATE.Repositories;
using REAL_STATE.Repositories.IRepositories;
using REAL_STATE.Services.Interfaces;

namespace REAL_STATE.Services
{
    public class ReferralService : IRefferalService
    {
        private readonly IReferralRepository _referralRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReferralService(UserManager<ApplicationUser> userManager,
            IReferralRepository referralRepository)
        {
            _userManager = userManager;
            _referralRepository = referralRepository;
        }

        public async Task<string> GenerateAsyncService(string userId)
        {
            var result = await _referralRepository.GenerateAsync(userId);

            return result;
        }


        public async Task<ReferralIndexVM> GetAllAndLatestReferralService(string userId, Func<string, string> buildLink)
        {
            var vm = await _referralRepository.GetAllAndLatestReferralService(userId, buildLink);

             return vm;
        }
    }
}
