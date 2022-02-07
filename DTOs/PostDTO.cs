using System;
using System.ComponentModel.DataAnnotations;

namespace BlogProject.DTOs
{
    public class PostDTO
    {
        public int Id { get; set; }

        [Display(Name = "Blog Name")]
        public string BlogName { get; set; }
        public string Author { get; set; }

        [Required]
        [StringLength(75, ErrorMessage = "The {0} must be at least {2} and no more than {1} characters", MinimumLength = 2)]
        public string Title { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and no more than {1} characters", MinimumLength = 2)]
        public string Abstract { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Created Date")]
        public DateTime Created { get; set; }

        public string Slug { get; set; }

        [Display(Name = "Blog Image")]
        public string ImageData { get; set; }

        [Display(Name = "Content Type")]
        public string ContentType { get; set; }
    }
}
