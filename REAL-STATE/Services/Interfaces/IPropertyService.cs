using REAL_STATE.Models;
using REAL_STATE.Models.DTO.PropertyDTO;
using REAL_STATE.Models.DTO.PropertyDTO.REAL_STATE.DTOs;
using REAL_STATE.Models.VM;

namespace REAL_STATE.Services.Interfaces
{
    public interface IPropertyService
    {
        Task<List<FilterPropertyDTO>> Filter(CriteriaDTO criteriaDTO);
        Task AddProperty(PropertyVM vm);
           
    }
}
