﻿using BlogProject.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogProject.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Display(Name = "Blog Name")]
        public int BlogId { get; set; }
        public string BlogUserId { get; set; }

        [Required]
        [StringLength(75, ErrorMessage = "The {0} must be at least {2} and no more than {1} characters", MinimumLength = 2)]
        public string Title { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and no more than {1} characters", MinimumLength = 2)]
        public string Abstract { get; set; }

        [Required]
        public string Content { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Created Date")]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Updated Date")]
        public DateTime? Updated { get; set; }

        public ReadyStatus ReadyStatus { get; set; }

        public string Slug { get; set; }

        [Display(Name = "Blog Image")]
        public byte[] ImageData { get; set; }

        [Display(Name = "Content Type")]
        public string ContentType { get; set; }

        [NotMapped]
        public IFormFile Image { get; set; }

        // Navigation properties
        public virtual Blog Blog { get; set; }

        [Display(Name = "Author")]
        public virtual BlogUser BlogUser { get; set; }
        public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }
}
