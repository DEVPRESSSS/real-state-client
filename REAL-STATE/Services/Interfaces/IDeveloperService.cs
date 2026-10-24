using REAL_STATE.Models;
using REAL_STATE.Models.DTO.PropertyDTO;
using REAL_STATE.Models.DTO.PropertyDTO.REAL_STATE.DTOs;
using REAL_STATE.Models.VM;

namespace REAL_STATE.Services.Interfaces
{
    public interface IDeveloperService
    {
        Task AddDeveloper(Developers developer);
        Task UpdateDeveloper(Developers developer);
        Task<List<DeveloperCardVM>> GetActiveDevelopers();
    }
}
