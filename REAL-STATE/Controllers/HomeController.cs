using Microsoft.AspNetCore.Mvc;
using REAL_STATE.Models;
using REAL_STATE.Models.DTO.PropertyDTO;
using REAL_STATE.Services.Interfaces;
using System.Diagnostics;

namespace REAL_STATE.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPropertyService _propertyService;
        public HomeController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }
    
        public async Task<IActionResult> Index([FromQuery] CriteriaDTO criteria)
        {
            var properties = await _propertyService.Filter(criteria);
            return View(properties);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
