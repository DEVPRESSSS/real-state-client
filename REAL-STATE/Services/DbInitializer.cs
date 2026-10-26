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
        public void Initialize()
        {
            if (_context.Database.GetPendingMigrations().Any())
            {
                _context.Database.Migrate();
            }

            if (!_roleManager.RoleExistsAsync(RoleSD.User).GetAwaiter().GetResult() || !_roleManager.RoleExistsAsync(RoleSD.Agent).GetAwaiter().GetResult())
            {
                // Create Roles
                _roleManager.CreateAsync(new IdentityRole(RoleSD.User)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(RoleSD.Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(RoleSD.Agent)).GetAwaiter().GetResult();

                var user = new ApplicationUser
                {
                    UserName = "clive@gmail.com",
                    Email = "clive@gmail.com",
                   
                    PhoneNumber = "09789446980",
                    EmailConfirmed = true
                };

                _userManager.CreateAsync(user, "Admin123*").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user, RoleSD.Admin).GetAwaiter().GetResult();
            }

        }
    }

}
