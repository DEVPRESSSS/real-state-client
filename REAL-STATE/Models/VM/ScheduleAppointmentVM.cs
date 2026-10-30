using System.ComponentModel.DataAnnotations;

namespace REAL_STATE.Models.VM
{
    public class ScheduleAppointmentVM
    {
        [Required]
        public string PropertyId { get; set; } = string.Empty;
        public string PropertyTitle { get; set; } = string.Empty;
        public string PropertyLocation { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ListingType { get; set; } = string.Empty;
        public string? PrimaryImageUrl { get; set; }
        public string? PrimaryImageAlt { get; set; }
        public string? AgentName { get; set; }

        [Required(ErrorMessage = "Please enter your full name.")]
        [StringLength(150)]
        [Display(Name = "Full name")]
        public string ContactName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(256)]
        [Display(Name = "Email")]
        public string ContactEmail { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(30)]
        [Display(Name = "Phone (optional)")]
        public string? ContactPhone { get; set; }

        [Required(ErrorMessage = "Please choose a preferred date.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Preferred date")]
        public DateTime? PreferredDate { get; set; }

        [Required(ErrorMessage = "Please choose a preferred time.")]
        [DataType(DataType.Time)]
        [Display(Name = "Preferred time")]
        public TimeOnly? PreferredTime { get; set; }

        [StringLength(1000, ErrorMessage = "Notes can be up to 1,000 characters.")]
        [Display(Name = "Notes (optional)")]
        public string? Notes { get; set; }
    }
}
