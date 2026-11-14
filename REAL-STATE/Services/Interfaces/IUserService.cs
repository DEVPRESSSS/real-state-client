using REAL_STATE.Models;
using REAL_STATE.Models.VM;

namespace REAL_STATE.Services.Interfaces
{
    public interface IUserService
    {

        Task<PagedResult<UserRowVM>> GetAllUsers(string currentUserId, int page = 1, int pageSize = 20, string? search = null);
        Task<UserVM?> GetUserForEdit(string id);
        Task AddUser(UserVM vm);
        Task UpdateUser(UserVM vm);
        Task SetLock(string id, bool locked, string currentUserId);
    }
}
