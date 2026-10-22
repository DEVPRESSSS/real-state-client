using REAL_STATE.Models;
using REAL_STATE.Models.DTO.PropertyDTO;
using REAL_STATE.Models.DTO.PropertyDTO.REAL_STATE.DTOs;
using REAL_STATE.Models.VM;

namespace REAL_STATE.Services.Interfaces
{
    public interface IAgentService
    {
        Task AddAgent(Agent agent);
        Task UpdateAgent(Agent agent);

      
    }
}
