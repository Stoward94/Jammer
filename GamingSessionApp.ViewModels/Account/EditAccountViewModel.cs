using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace GamingSessionApp.ViewModels.Account
{
    public class EditAccountViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        public SelectList TimeZones { get; set; }

        [Required]
        public string TimeZoneId { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
