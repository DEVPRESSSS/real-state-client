using System.ComponentModel.DataAnnotations;

namespace REAL_STATE.Models.VM
{
    public class AgentRegisterVM
    {
        [Required, StringLength(150)]
        [Display(Name = "Full name")]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [Phone, StringLength(30)]
        public string? Phone { get; set; }

        [Required, StringLength(50)]
        [Display(Name = "License number")]
        public string LicenseNumber { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Bio { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}