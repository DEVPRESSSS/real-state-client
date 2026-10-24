using REAL_STATE.Data;
using REAL_STATE.Models;
using REAL_STATE.Repositories.IRepositories;

namespace REAL_STATE.Repositories
{
    public class UserRepository: Repository<ApplicationUser>, IUserRepository
    {
        public UserRepository(ApplicationDbContext db)
            : base(db)
        {
        }
    }
}
