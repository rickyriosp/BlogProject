using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BlogProject.Data;
using BlogProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using BlogProject.Enums;

namespace BlogProject.Services
{
    public class BlogSearchService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public BlogSearchService(ApplicationDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        public IQueryable<Post> Search(string searchTerm)
        {
            var posts = _context.Posts.Include(p => p.Comments).Where(p => p.ReadyStatus == Enums.ReadyStatus.ProductionReady).AsQueryable();

            if (searchTerm is not null)
            {
                searchTerm = searchTerm.ToLower();
                posts = posts.Where(
                    p => p.Title.ToLower().Contains(searchTerm) ||
                    p.Abstract.ToLower().Contains(searchTerm) ||
                    p.Content.ToLower().Contains(searchTerm) ||
                    p.Comments.Any(c => c.Body.ToLower().Contains(searchTerm) ||
                                        c.ModeratedBody.ToLower().Contains(searchTerm) ||
                                        c.BlogUser.FirstName.ToLower().Contains(searchTerm) ||
                                        c.BlogUser.LastName.ToLower().Contains(searchTerm) ||
                                        c.BlogUser.Email.ToLower().Contains(searchTerm)));
            }

            return posts.OrderByDescending(p => p.Created);
        }

        public IQueryable<Post> SearchTag(string searchTerm)
        {
            IQueryable<Post> posts;
            var currentUser = _contextAccessor.HttpContext.User;

            if (currentUser.IsInRole(BlogRole.Administrator.ToString()))
            {
                posts = _context.Posts.Include(p => p.Comments).AsQueryable();
            }
            else
            {
                posts = _context.Posts.Include(p => p.Comments).Where(p => p.ReadyStatus == Enums.ReadyStatus.ProductionReady).AsQueryable();
            }

            if (searchTerm is not null)
            {
                searchTerm = searchTerm.ToLower();
                posts = posts.Where(p => p.Tags.Any(t => t.Text.ToLower().Contains(searchTerm)));
            }

            return posts.OrderByDescending(p => p.Created);
        }
    }
}
