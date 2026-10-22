using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace REAL_STATE.Models.VM
{
    public class PropertyVM
    {
        public Property Property { get; set; } = new();
        [ValidateNever]
        public bool IsEdit { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> Developers { get; set; } = new List<SelectListItem>();

        [ValidateNever]
        public IEnumerable<SelectListItem> Agents { get; set; } = new List<SelectListItem>();
    }
}