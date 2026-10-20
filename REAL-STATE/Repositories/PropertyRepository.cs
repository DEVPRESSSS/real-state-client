using REAL_STATE.Data;
using REAL_STATE.Models;
using REAL_STATE.Repositories.IRepositories;

namespace REAL_STATE.Repositories
{
    public class PropertyRepository: Repository<Property>, IPropertyRepository
    {
        public PropertyRepository(ApplicationDbContext db)
            : base(db)
        {
        }
    }
}
