using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace REAL_STATE.Models.VM
{
    public class UserVM
    {
        public string? Id { get; set; }

        [Required, StringLength(150)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [Phone, StringLength(30)]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please select a role.")]
        public string Role { get; set; } = string.Empty;

        // Only required when creating (checked in the controller)
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }

        [ValidateNever]
        public bool IsEdit { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> Roles { get; set; } = new List<SelectListItem>();
    }
}
