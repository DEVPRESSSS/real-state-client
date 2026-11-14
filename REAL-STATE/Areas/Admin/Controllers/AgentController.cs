using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using REAL_STATE.Repositories.IRepositories;
using REAL_STATE.Services;
using REAL_STATE.Services.Interfaces;
using REAL_STATE.Utilities;
using REAL_STATE.Models;

namespace REAL_STATE.Areas.Admin.Controllers
{
    [Area(RoleSD.Admin)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    [Authorize(Roles = RoleSD.Admin)]
    public class AgentController : Controller
    {
        private readonly REAL_STATE.Utilities.SafeErrorMessages _errors;
        private readonly IAgentRepository _agentRepository;
        private readonly IAgentService _agentService;

        public AgentController(
            IAgentRepository agentRepository,
            IAgentService agentService,
            SafeErrorMessages errors)
        {
            _errors = errors;
            _agentRepository = agentRepository;
            _agentService = agentService;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 20, string? q = null)
        {
            q = q?.Trim();
            var result = await _agentRepository.GetPageAsync(
                query => query.OrderBy(x => x.FullName).ThenBy(x => x.Id), page, pageSize,
                x => q == null || q == "" || x.FullName.Contains(q) || x.Email.Contains(q), includeProperties: "");
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(string? id)
        {

            if (!string.IsNullOrEmpty(id))
            {
                var agent = await _agentRepository.GetAsync(x => x.Id == id);
                if (agent == null)
                    return NotFound();
                return View(agent);

            }

            return View(new Agent());
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Agent agent)
        {
            if (!ModelState.IsValid)
            {
                var agentExist = await _agentRepository.GetAsync(x => x.Id == agent.Id);
                return View(agentExist);
            }

            try
            {
                var existing = await _agentRepository.GetAsync(x => x.Id == agent.Id);

                if (existing == null)
                {
                    await _agentService.AddAgent(agent);
                    TempData["SuccessMessage"] = "Agent added successfully.";
                }
                else
                {
                    await _agentService.UpdateAgent(agent);
                    TempData["SuccessMessage"] = "Agent updated successfully.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = _errors.For(ex);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var agent = await _agentRepository.GetAsync(x => x.Id == id);
            if (agent == null)
                return NotFound(new { success = false, message = "Agent not found." });

            try
            {
                 _agentRepository.Remove(agent);
                await _agentRepository.Save();
                return Json(new { success = true, message = "Agent deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = _errors.For(ex) });
            }
        }

    }
}
