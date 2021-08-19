using System.ComponentModel.DataAnnotations;

namespace BlogProject.ViewModels
{
    public class ContactMe
    {
        [Required]
        [StringLength(80, ErrorMessage = "The {0} must be at least {2} and no more than {1} characters", MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name="Email Address")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and no more than {1} characters", MinimumLength = 5)]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and no more than {1} characters", MinimumLength = 2)]
        public string Subject { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "The {0} must be at least {2} and no more than {1} characters", MinimumLength = 2)]
        public string Message { get; set; }
    }
}
