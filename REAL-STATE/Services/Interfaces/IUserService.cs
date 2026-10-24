using REAL_STATE.Models;
using REAL_STATE.Models.VM;

namespace REAL_STATE.Services.Interfaces
{
    public interface IUserService
    {

        Task<List<UserRowVM>> GetAllUsers(string currentUserId);
        Task<UserVM?> GetUserForEdit(string id);
        Task AddUser(UserVM vm);
        Task UpdateUser(UserVM vm);
        Task SetLock(string id, bool locked, string currentUserId);
    }
}
