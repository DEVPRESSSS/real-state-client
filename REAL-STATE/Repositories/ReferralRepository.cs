using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using REAL_STATE.Data;
using REAL_STATE.Models;
using REAL_STATE.Models.VM;
using REAL_STATE.Repositories.IRepositories;
using System.Security.Cryptography;

namespace REAL_STATE.Repositories
{
    public class ReferralRepository: Repository<Referral>, IReferralRepository
    {
        public ReferralRepository(ApplicationDbContext db)
            : base(db)
        {
        }

        public async Task<string> GenerateAsync(string userId)
        {
            string code;
            do
            {
                code = RandomNumberGenerator.GetString("ABCDEFGHJKLMNPQRSTUVWXYZ23456789", 8);
            }
            while (await _db.Referrals.AnyAsync(r => r.ReferralCode == code));

            _db.Referrals.Add(new Referral
            {
                ReferrerUserId = userId,
                ReferralCode = code,
                Status = "Pending"
            });
            await _db.SaveChangesAsync();

            return code;
        }

        public async Task<ReferralIndexVM> GetAllAndLatestReferralService(string userId, Func<string, string> buildLink)
        {

            var rows = await _db.Referrals
                .AsNoTracking()
                .Include(r => r.ReferredUser)
                .Where(r => r.ReferrerUserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var vm = new ReferralIndexVM
            {
                Referrals = rows.Select(r => new ReferralRowVM
                {
                    Id = r.Id,
                    Code = r.ReferralCode,
                    Link = buildLink(r.ReferralCode),
                    Status = r.Status,
                    ReferredName = r.ReferredUser?.FullName,
                    CreatedAt = r.CreatedAt
                }).ToList()
            };

            vm.Latest = vm.Referrals.FirstOrDefault(r => r.Status == "Pending");

            return vm;
        }
        public async Task<bool> RedeemAsync(string code, string newUserId)
        {
           
            var rows = await _db.Referrals
                .Where(r => r.ReferralCode == code
                         && r.ReferredUserId == null
                         && r.ReferrerUserId != newUserId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(r => r.ReferredUserId, newUserId)
                    .SetProperty(r => r.Status, "Registered"));

            return rows > 0;
        }

    }
}
