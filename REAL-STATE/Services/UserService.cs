using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using REAL_STATE.Models;
using REAL_STATE.Models.VM;
using REAL_STATE.Services.Interfaces;
using REAL_STATE.Utilities;

namespace REAL_STATE.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<UserRowVM>> GetAllUsers(string currentUserId)
        {
            var users = await _userManager.Users.OrderBy(x => x.Email).ToListAsync();
            var rows = new List<UserRowVM>();

            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);

                rows.Add(new UserRowVM
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email ?? "",
                    PhoneNumber = u.PhoneNumber,
                    Role = roles.FirstOrDefault() ?? "-",
                    IsLocked = u.LockoutEnd.HasValue && u.LockoutEnd.Value > DateTimeOffset.UtcNow,
                    IsCurrentUser = u.Id == currentUserId
                });
            }

            return rows;
        }

        public async Task<UserVM?> GetUserForEdit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new UserVM
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber,
                Role = roles.FirstOrDefault() ?? "",
                IsEdit = true
            };
        }

        public async Task AddUser(UserVM vm)
        {
            if (await _userManager.FindByEmailAsync(vm.Email) != null)
                throw new GlobalExceptionHandler("Email is already in use.");

            var user = new ApplicationUser
            {
                UserName = vm.Email,
                Email = vm.Email,
                FullName = vm.FullName,
                PhoneNumber = vm.PhoneNumber,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, vm.Password!);
            if (!result.Succeeded)
                throw new GlobalExceptionHandler(string.Join(" ", result.Errors.Select(e => e.Description)));

            var roleResult = await _userManager.AddToRoleAsync(user, vm.Role);
            if (!roleResult.Succeeded)
                throw new GlobalExceptionHandler(string.Join(" ", roleResult.Errors.Select(e => e.Description)));
        }

        public async Task UpdateUser(UserVM vm)
        {
            var user = await _userManager.FindByIdAsync(vm.Id!);
            if (user is null)
                throw new GlobalExceptionHandler("User not found!!");

            var emailOwner = await _userManager.FindByEmailAsync(vm.Email);
            if (emailOwner != null && emailOwner.Id != user.Id)
                throw new GlobalExceptionHandler("Email is already in use.");

            user.FullName = vm.FullName;
            user.PhoneNumber = vm.PhoneNumber;

            if (!string.Equals(user.Email, vm.Email, StringComparison.OrdinalIgnoreCase))
            {
                user.Email = vm.Email;
                user.UserName = vm.Email;
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new GlobalExceptionHandler(string.Join(" ", result.Errors.Select(e => e.Description)));

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (!currentRoles.Contains(vm.Role) || currentRoles.Count > 1)
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, vm.Role);
            }

            if (!string.IsNullOrWhiteSpace(vm.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var pwResult = await _userManager.ResetPasswordAsync(user, token, vm.Password);
                if (!pwResult.Succeeded)
                    throw new GlobalExceptionHandler(string.Join(" ", pwResult.Errors.Select(e => e.Description)));
            }
        }

        public async Task SetLock(string id, bool locked, string currentUserId)
        {
            if (id == currentUserId)
                throw new GlobalExceptionHandler("You cannot lock your own account.");

            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                throw new GlobalExceptionHandler("User not found!!");

            await _userManager.SetLockoutEnabledAsync(user, true);
            await _userManager.SetLockoutEndDateAsync(user, locked ? DateTimeOffset.MaxValue : null);

            if (!locked)
                await _userManager.ResetAccessFailedCountAsync(user);
        }
    }
}