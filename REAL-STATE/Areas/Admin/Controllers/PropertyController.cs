using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using REAL_STATE.Data;
using REAL_STATE.Models.DTO.PropertyDTO;
using REAL_STATE.Services.Interfaces;
using REAL_STATE.Utilities;

namespace REAL_STATE.Areas.Admin.Controllers
{
    [Area(RoleSD.Admin)]
    [Authorize(Roles = RoleSD.Admin)]
    public class PropertyController : Controller
    {
        private readonly IPropertyService _propertyService;
        public PropertyController(
            IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        public IActionResult Upsert()
        {
            return View();
        }
    
    }
}
