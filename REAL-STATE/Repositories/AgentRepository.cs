using REAL_STATE.Data;
using REAL_STATE.Models;
using REAL_STATE.Repositories.IRepositories;

namespace REAL_STATE.Repositories
{
    public class AgentRepository: Repository<Agent>, IAgentRepository
    {
        public AgentRepository(ApplicationDbContext db)
            : base(db)
        {
        }
    }
}
