using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using REAL_STATE.Data;
using REAL_STATE.Models;
using REAL_STATE.Services.Interfaces;
using REAL_STATE.Utilities;

namespace REAL_STATE.Services
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task InitializeAsync()
        {
            if ((await _context.Database.GetPendingMigrationsAsync()).Any())
            {
                await _context.Database.MigrateAsync();
            }

            var seedAdmin = !await _roleManager.RoleExistsAsync(RoleSD.User)
                || !await _roleManager.RoleExistsAsync(RoleSD.Agent);
            foreach (var role in new[] { RoleSD.User, RoleSD.Admin, RoleSD.Agent })
                if (!await _roleManager.RoleExistsAsync(role))
                    (await _roleManager.CreateAsync(new IdentityRole(role))).ThrowIfFailed();

            if (seedAdmin)
            {
                var user = await _userManager.FindByEmailAsync("clive@gmail.com");
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = "clive@gmail.com", Email = "clive@gmail.com",
                        PhoneNumber = "09789446980", EmailConfirmed = true
                    };
                    // P0 bootstrap credentials are preserved at the user's explicit request.
                    (await _userManager.CreateAsync(user, "Admin123*")).ThrowIfFailed();
                }
                if (!await _userManager.IsInRoleAsync(user, RoleSD.Admin))
                    (await _userManager.AddToRoleAsync(user, RoleSD.Admin)).ThrowIfFailed();
            }
        }
    }
}
