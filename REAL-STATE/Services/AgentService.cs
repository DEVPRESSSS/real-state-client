using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AgentService(
            IAgentRepository agentRepository,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            _agentRepository = agentRepository;
            _userManager = userManager;
            _roleManager = roleManager;
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

        public async Task<PagedAgentsVM> GetAgentsPage(int page, int pageSize = 10)
        {
            var agents = (await _agentRepository.GetAllAsync(
                    x => x.IsActive,
                    includeProperties: "Properties"))
                .OrderBy(x => x.FullName)
                .ToList();

            var totalItems = agents.Count;
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
            page = Math.Clamp(page, 1, totalPages);

            var cards = agents
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a =>
                {
                    var available = a.Properties.Where(p => p.Status == "Available").ToList();

                    var area = available
                        .Where(p => !string.IsNullOrWhiteSpace(p.City))
                        .GroupBy(p => p.City)
                        .OrderByDescending(g => g.Count())
                        .Take(2)
                        .Select(g => g.Key)
                        .ToList();

                    return new AgentCardVM
                    {
                        Id = a.Id,
                        FullName = a.FullName,
                        Role = a.Role,
                        PhotoUrl = a.PhotoUrl,
                        Email = a.Email,
                        IsVerified = !string.IsNullOrWhiteSpace(a.LicenseNumber),
                        Area = area.Any() ? string.Join(" & ", area) : "-",
                        ActiveListings = available.Count
                    };
                })
                .ToList();

            return new PagedAgentsVM
            {
                Agents = cards,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }

        public async Task RegisterAgent(AgentRegisterVM vm)
        {
            if (await _userManager.FindByEmailAsync(vm.Email) != null)
                throw new GlobalExceptionHandler("Email is already registered.");

            var emailUsedByAgent = (await _agentRepository.GetAllAsync(x => x.Email == vm.Email)).Any();
            if (emailUsedByAgent)
                throw new GlobalExceptionHandler("An agent with this email already exists.");

            if (!await _roleManager.RoleExistsAsync(RoleSD.Agent))
                await _roleManager.CreateAsync(new IdentityRole(RoleSD.Agent));

            var user = new ApplicationUser
            {
                UserName = vm.Email,
                Email = vm.Email,
                FullName = vm.FullName,
                PhoneNumber = vm.Phone,
                EmailConfirmed = true   
            };

            var result = await _userManager.CreateAsync(user, vm.Password);
            if (!result.Succeeded)
                throw new GlobalExceptionHandler(string.Join(" ", result.Errors.Select(e => e.Description)));

            try
            {
                await _userManager.AddToRoleAsync(user, RoleSD.Agent);

                var agent = new Agent
                {
                    UserId = user.Id,
                    FullName = vm.FullName,
                    Email = vm.Email,
                    Phone = vm.Phone,
                    LicenseNumber = vm.LicenseNumber,
                    Bio = vm.Bio,
                    Role = "Agent",
                    IsActive = false   
                };

                await _agentRepository.AddAsync(agent);
                await _agentRepository.Save();
            }
            catch
            {
                await _userManager.DeleteAsync(user);
                throw;
            }
        }
    }
}
