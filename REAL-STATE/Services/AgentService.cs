using Microsoft.EntityFrameworkCore;
using REAL_STATE.Data;
using REAL_STATE.Models;
using REAL_STATE.Models.DTO.PropertyDTO;
using REAL_STATE.Models.DTO.PropertyDTO.REAL_STATE.DTOs;
using REAL_STATE.Models.VM;
using REAL_STATE.Repositories.IRepositories;
using REAL_STATE.Services.Interfaces;
using REAL_STATE.Utilities;

namespace REAL_STATE.Services
{
    public class AgentService:IAgentService
    {
        private readonly IAgentRepository _agentRepository;

        public AgentService(
            IAgentRepository agentRepository

            )
        {
            _agentRepository = agentRepository;
        }

        public async Task AddAgent(Agent agent)
        {
            await _agentRepository.AddAsync(agent);
            await _agentRepository.Save();
        }
        public async Task UpdateAgent(Agent agent)
        {
            var agentObj = await _agentRepository.GetAsync(x => x.Id == agent.Id);
            if (agentObj is null)
                throw new GlobalExceptionHandler("Agent not found!!");

            agentObj.FullName = agent.FullName;
            agentObj.Email = agent.Email;
            agentObj.Phone = agent.Phone;
            agentObj.Bio = agent.Bio;
            agentObj.LicenseNumber = agent.LicenseNumber;
            agentObj.Role = agent.Role;
            agentObj.IsActive = agent.IsActive;

            if (!string.IsNullOrWhiteSpace(agent.PhotoUrl))
                agentObj.PhotoUrl = agent.PhotoUrl;

            await _agentRepository.UpdateAsync(agentObj);
            await _agentRepository.Save();
        }


    }
}
