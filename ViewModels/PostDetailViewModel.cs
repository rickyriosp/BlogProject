using BlogProject.Models;
using System.Collections.Generic;

namespace BlogProject.ViewModels
{
    public class PostDetailView
    {
        public Post Post { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }
}
