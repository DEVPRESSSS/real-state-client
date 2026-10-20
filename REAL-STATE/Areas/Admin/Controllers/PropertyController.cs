using Microsoft.AspNetCore.Mvc;
using REAL_STATE.Data;
using REAL_STATE.Models.DTO.PropertyDTO;
using REAL_STATE.Repository.Interfaces;

namespace REAL_STATE.Areas.Admin.Controllers
{
    public class PropertyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPropertyService _propertyService;
        public PropertyController(ApplicationDbContext context,
            IPropertyService propertyService)
        {
            _context = context;
            _propertyService = propertyService;
        }
    
    }
}
