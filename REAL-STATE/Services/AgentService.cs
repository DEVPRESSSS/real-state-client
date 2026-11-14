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
        private readonly ApplicationDbContext _db;
        private readonly IAccountConfirmationService _confirmation;
        private readonly UserManager<ApplicationUser> _userManager;
        public AgentService(
            IAgentRepository agentRepository,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext db, IAccountConfirmationService confirmation
            )
        {
            _agentRepository = agentRepository;
            _db = db;
            _confirmation = confirmation;
            _userManager = userManager;
        }

        public async Task AddAgent(Agent agent)
        {
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(agent,
                new System.ComponentModel.DataAnnotations.ValidationContext(agent), true);
            await _agentRepository.AddAsync(new Agent { FullName = agent.FullName, Email = agent.Email,
                Phone = agent.Phone, Bio = agent.Bio, LicenseNumber = agent.LicenseNumber,
                Role = agent.Role, IsActive = agent.IsActive, PhotoUrl = agent.PhotoUrl });
            await _agentRepository.Save();
        }
        public async Task UpdateAgent(Agent agent)
        {
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(agent,
                new System.ComponentModel.DataAnnotations.ValidationContext(agent), true);
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
            var result = await _db.Agents.AsNoTracking().Where(a => a.IsActive)
                .OrderBy(a => a.FullName).ThenBy(a => a.Id).Select(a => new AgentCardVM
                {
                    Id = a.Id, FullName = a.FullName, Role = a.Role, PhotoUrl = a.PhotoUrl,
                    Email = a.Email, IsVerified = a.LicenseNumber != null && a.LicenseNumber != "",
                    ActiveListings = a.Properties.Count(p => p.Status == "Available")
                }).ToPageAsync(page, pageSize);
            var cards = result.Items.ToList();
            var ids = cards.Select(a => a.Id).ToList();
            var areas = await _db.Properties.AsNoTracking()
                .Where(p => p.AgentId != null && ids.Contains(p.AgentId) && p.Status == "Available" && p.City != "")
                .GroupBy(p => new { p.AgentId, p.City })
                .Select(g => new { g.Key.AgentId, g.Key.City, Count = g.Count() }).ToListAsync();
            foreach (var card in cards)
            {
                var cities = areas.Where(a => a.AgentId == card.Id).OrderByDescending(a => a.Count)
                    .ThenBy(a => a.City).Take(2).Select(a => a.City).ToList();
                card.Area = cities.Count == 0 ? "-" : string.Join(" & ", cities);
            }
            return new PagedAgentsVM { Agents = cards, Page = result.CurrentPage,
                PageSize = result.PageSize, TotalItems = result.TotalItems };
        }

        public async Task RegisterAgent(AgentRegisterVM vm)
        {
            if (await _userManager.FindByEmailAsync(vm.Email) != null)
                throw new GlobalExceptionHandler("Email is already registered.");

            var emailUsedByAgent = await _agentRepository.GetAsync(x => x.Email == vm.Email) != null;
            if (emailUsedByAgent)
                throw new GlobalExceptionHandler("An agent with this email already exists.");

            await using var transaction = await _db.Database.BeginTransactionAsync();

            var user = new ApplicationUser
            {
                UserName = vm.Email,
                Email = vm.Email,
                FullName = vm.FullName,
                PhoneNumber = vm.Phone,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, vm.Password);
            if (!result.Succeeded)
                throw new GlobalExceptionHandler(string.Join(" ", result.Errors.Select(e => e.Description)));

            try
            {
                (await _userManager.AddToRoleAsync(user, RoleSD.Agent)).ThrowIfFailed();

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
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            await _confirmation.SendAsync(user);
        }
    }
}
