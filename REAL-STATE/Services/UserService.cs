using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using REAL_STATE.Data;
using REAL_STATE.Models;
using REAL_STATE.Models.VM;
using REAL_STATE.Services.Interfaces;
using REAL_STATE.Utilities;

namespace REAL_STATE.Services;

public sealed class UserService(UserManager<ApplicationUser> users, RoleManager<IdentityRole> roles,
    ApplicationDbContext db, IAccountConfirmationService confirmation) : IUserService
{
    public Task<PagedResult<UserRowVM>> GetAllUsers(string currentUserId, int page = 1, int pageSize = 20, string? search = null)
    {
        var now = DateTimeOffset.UtcNow;
        var query = db.Users.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(u => u.FullName.Contains(search) || (u.Email != null && u.Email.Contains(search)));
        return query.OrderBy(u => u.Email).ThenBy(u => u.Id).Select(u => new UserRowVM
        {
            Id = u.Id, FullName = u.FullName, Email = u.Email ?? "", PhoneNumber = u.PhoneNumber,
            Role = (from membership in db.UserRoles join role in db.Roles on membership.RoleId equals role.Id
                    where membership.UserId == u.Id orderby role.Name select role.Name).FirstOrDefault() ?? "-",
            IsLocked = u.LockoutEnd.HasValue && u.LockoutEnd > now, IsCurrentUser = u.Id == currentUserId
        }).ToPageAsync(page, pageSize);
    }

    public async Task<UserVM?> GetUserForEdit(string id)
    {
        var user = await users.FindByIdAsync(id);
        if (user is null) return null;
        var assigned = await users.GetRolesAsync(user);
        return new UserVM { Id = user.Id, FullName = user.FullName, Email = user.Email ?? "",
            PhoneNumber = user.PhoneNumber, Role = assigned.FirstOrDefault() ?? "", IsEdit = true };
    }

    public async Task AddUser(UserVM vm)
    {
        await ValidateAsync(vm);
        await using var transaction = await db.Database.BeginTransactionAsync();
        var user = new ApplicationUser { UserName = vm.Email, Email = vm.Email,
            FullName = vm.FullName, PhoneNumber = vm.PhoneNumber, EmailConfirmed = false };
        (await users.CreateAsync(user, vm.Password!)).ThrowIfFailed();
        (await users.AddToRoleAsync(user, vm.Role)).ThrowIfFailed();
        await transaction.CommitAsync();
        await confirmation.SendAsync(user);
    }

    public async Task UpdateUser(UserVM vm)
    {
        await ValidateAsync(vm);
        await using var transaction = await db.Database.BeginTransactionAsync();
        var user = await users.FindByIdAsync(vm.Id!) ?? throw new GlobalExceptionHandler("User not found.");
        var emailChanged = !string.Equals(user.Email, vm.Email, StringComparison.OrdinalIgnoreCase);
        user.FullName = vm.FullName;
        user.PhoneNumber = vm.PhoneNumber;
        if (emailChanged)
        {
            user.Email = vm.Email;
            user.UserName = vm.Email;
            user.EmailConfirmed = false;
        }
        (await users.UpdateAsync(user)).ThrowIfFailed();
        var assigned = await users.GetRolesAsync(user);
        if (!assigned.Contains(vm.Role) || assigned.Count > 1)
        {
            (await users.RemoveFromRolesAsync(user, assigned)).ThrowIfFailed();
            (await users.AddToRoleAsync(user, vm.Role)).ThrowIfFailed();
        }
        if (!string.IsNullOrWhiteSpace(vm.Password))
        {
            var token = await users.GeneratePasswordResetTokenAsync(user);
            (await users.ResetPasswordAsync(user, token, vm.Password)).ThrowIfFailed();
        }
        (await users.UpdateSecurityStampAsync(user)).ThrowIfFailed();
        await transaction.CommitAsync();
        if (emailChanged) await confirmation.SendAsync(user);
    }

    public async Task SetLock(string id, bool locked, string currentUserId)
    {
        if (id == currentUserId) throw new GlobalExceptionHandler("You cannot lock your own account.");
        await using var transaction = await db.Database.BeginTransactionAsync();
        var user = await users.FindByIdAsync(id) ?? throw new GlobalExceptionHandler("User not found.");
        (await users.SetLockoutEnabledAsync(user, true)).ThrowIfFailed();
        (await users.SetLockoutEndDateAsync(user, locked ? DateTimeOffset.MaxValue : null)).ThrowIfFailed();
        if (!locked) (await users.ResetAccessFailedCountAsync(user)).ThrowIfFailed();
        (await users.UpdateSecurityStampAsync(user)).ThrowIfFailed();
        await transaction.CommitAsync();
    }

    private async Task ValidateAsync(UserVM vm)
    {
        if (!await roles.RoleExistsAsync(vm.Role)) throw new GlobalExceptionHandler("Select a valid role.");
        var owner = await users.FindByEmailAsync(vm.Email);
        if (owner != null && owner.Id != vm.Id) throw new GlobalExceptionHandler("Email is already in use.");
    }
}
