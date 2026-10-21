using REAL_STATE.Data;
using REAL_STATE.Models;
using REAL_STATE.Repositories.IRepositories;

namespace REAL_STATE.Repositories
{
    public class DeveloperRepository: Repository<Developers>, IDeveloperRepository
    {
        public DeveloperRepository(ApplicationDbContext db)
            : base(db)
        {
        }
    }
}
