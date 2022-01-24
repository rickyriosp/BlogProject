using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogProject.Data;
using BlogProject.Models;
using BlogProject.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using X.PagedList;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace BlogProject.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISlugService _slugService;
        private readonly IImageService _imageService;
        private readonly UserManager<BlogUser> _userManager;
        private readonly BlogSearchService _blogSearchService;
        private readonly IConfiguration _configuration;

        public PostsController(ApplicationDbContext context, ISlugService slugService, IImageService imageService, UserManager<BlogUser> userManager, BlogSearchService blogSearchService, IConfiguration configuration)
        {
            _context = context;
            _slugService = slugService;
            _imageService = imageService;
            _userManager = userManager;
            _blogSearchService = blogSearchService;
            _configuration = configuration;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Posts.Include(p => p.Blog).Include(p => p.BlogUser);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> BlogPostIndex(int? id, int? page)
        {
            if (id is null)
            {
                return NotFound();
            }

            var pageNumber = page ?? 1;
            var pageSize = 5;

            var blog = _context.Blogs.Find(id);

            var posts = _context.Posts
                .Where(p => p.BlogId == id)
                .Where(p => p.ReadyStatus == Enums.ReadyStatus.ProductionReady)
                .Include(p => p.Blog)
                .OrderByDescending(p => p.Created)
                .ToPagedListAsync(pageNumber, pageSize);

            ViewData["HeaderImage"] = _imageService.DecodeImage(blog.ImageData, blog.ContentType);
            ViewData["MainText"] = blog.Name;
            ViewData["SubText"] = blog.Description;

            return View(await posts);
        }

        public async Task<IActionResult> SearchIndex(int? page, string searchTerm)
        {
            ViewData["SearchTerm"] = searchTerm;

            var pageNumber = page ?? 1;
            var pageSize = 5;

            var posts = _blogSearchService.Search(searchTerm);

            return View(await posts.ToPagedListAsync(pageNumber, pageSize));
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Blog)
                .Include(p => p.BlogUser)
                .Include(p => p.Tags)
                .Include(p => p.Comments)
                .ThenInclude(c => c.BlogUser)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Moderator)
                .FirstOrDefaultAsync(m => m.Slug == slug);

            if (post == null)
            {
                return NotFound();
            }

            var dataVM = new ViewModels.PostDetailView()
            {
                Post = post,
                Tags = _context.Tags
                        .Select(t => t.Text.ToLower())
                        .Distinct().ToList()
            };

            ViewData["HeaderImage"] = _imageService.DecodeImage(post.ImageData, post.ContentType);
            ViewData["MainText"] = post.Title;
            ViewData["SubText"] = post.Abstract;

            return View(dataVM);
        }

        // GET: Posts/Create
        [Authorize(Roles = "Administrator, GuestAuthor")]
        public IActionResult Create()
        {
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name");
            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BlogId,Title,Abstract,Content,ReadyStatus,Image")] Post post, List<string> tagValues)
        {
            if (ModelState.IsValid)
            {
                post.Created = DateTime.Now.ToUniversalTime();

                var authorId = _userManager.GetUserId(User);
                post.BlogUserId = authorId;

                // Use the _imageService to store the incoming user specified image
                post.ImageData = (await _imageService.EncodeImageAsync(post.Image) ??
                                  await _imageService.EncodeImageAsync(_configuration["DefaultPostImage"]));
                post.ContentType = post.Image is null ?
                                    Path.GetExtension(_configuration["DefaultPostImage"]) :
                                    _imageService.ContentType(post.Image);

                // Create the slug and determine if it is unique
                var slug = _slugService.UrlFriendly(post.Title);

                // Create a variable to store whether an error has ocurred
                var validationError = false;

                if (string.IsNullOrEmpty(slug))
                {
                    // Add a Model state error and return the user back to the Create View
                    ModelState.AddModelError("", "The Title you provided cannot be used as it results in an empty slug.");
                    validationError = true;
                }

                if (!_slugService.IsUnique(slug))
                {
                    // Add a Model state error and return the user back to the Create View
                    ModelState.AddModelError("", "The Title you provided cannot be used as it results in a duplicate slug.");
                    validationError = true;
                }

                if (validationError)
                {
                    ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", post.BlogId);
                    ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id", post.BlogUserId);
                    ViewData["TagValues"] = string.Join(",", tagValues);
                    return View(post);
                }

                post.Slug = slug;

                _context.Add(post);
                await _context.SaveChangesAsync();

                foreach (var tag in tagValues)
                {
                    _context.Add(new Tag()
                    {
                        PostId = post.Id,
                        BlogUserId = authorId,
                        Text = tag,
                    });
                }
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", post.BlogId);
            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id", post.BlogUserId);
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", post.BlogId);
            ViewData["TagValues"] = string.Join(",", post.Tags.Select(t => t.Text));

            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogId,Title,Abstract,Content,ReadyStatus")] Post post, IFormFile newImage, List<string> tagValues)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // The original post
                    var newPost = await _context.Posts
                        .Include(p => p.Tags)
                        .FirstOrDefaultAsync(p => p.Id == post.Id);

                    newPost.Updated = DateTime.Now.ToUniversalTime();

                    if (newPost.BlogId != post.BlogId)
                    {
                        newPost.BlogId = post.BlogId;
                    }
                    if (newPost.Title != post.Title)
                    {
                        var newSlug = _slugService.UrlFriendly(post.Title);

                        if (newSlug != newPost.Slug)
                        {
                            if (_slugService.IsUnique(newSlug))
                            {
                                newPost.Title = post.Title;
                                newPost.Slug = newSlug;
                            }
                            else
                            {
                                ModelState.AddModelError("", "The Title you provided cannot be used as it results in a duplicate slug.");
                                ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", post.BlogId);
                                ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id", post.BlogUserId);
                                ViewData["TagValues"] = string.Join(",", tagValues);
                                return View(post);
                            }
                        }
                    }
                    if (newPost.Abstract != post.Abstract)
                    {
                        newPost.Abstract = post.Abstract;
                    }
                    if (newPost.Content != post.Content)
                    {
                        newPost.Content = post.Content;
                    }
                    if (newPost.ReadyStatus != post.ReadyStatus)
                    {
                        newPost.ReadyStatus = post.ReadyStatus;
                    }
                    if (newImage is not null)
                    {
                        newPost.ImageData = await _imageService.EncodeImageAsync(newImage);
                        newPost.ContentType = _imageService.ContentType(newImage);
                    }

                    // Remove all Tags associated with this Post
                    _context.Tags.RemoveRange(newPost.Tags);

                    // Add in the new Tags from the Edit form
                    foreach (var tag in tagValues)
                    {
                        _context.Add(new Tag()
                        {
                            PostId = post.Id,
                            BlogUserId = newPost.BlogUserId,
                            Text = tag
                        });
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", post.BlogId);

            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Blog)
                .Include(p => p.BlogUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
