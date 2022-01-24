using BlogProject.Data;
using BlogProject.Enums;
using BlogProject.Models;
using BlogProject.Services;
using BlogProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace BlogProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBlogEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, IBlogEmailSender emailSender, ApplicationDbContext context)
        {
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        public async Task<IActionResult> Index(int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 5;

            var blogs = _context.Blogs
               .Where(b => b.Posts.Any(p => p.ReadyStatus == Enums.ReadyStatus.ProductionReady))
               .Include(b => b.BlogUser)
               .OrderByDescending(b => b.Created)
               .ToPagedListAsync(pageNumber, pageSize);

            if (User.IsInRole(BlogRole.Administrator.ToString()) || User.IsInRole(BlogRole.GuestAuthor.ToString()))
            {
                blogs = _context.Blogs
                    .Include(b => b.BlogUser)
                    .OrderByDescending(b => b.Created)
                    .ToPagedListAsync(pageNumber, pageSize);
            }

            ViewData["HeaderImage"] = "/assets/img/home-bg.jpg";
            ViewData["MainText"] = "Ricky's Blog";
            ViewData["SubText"] = "This is my blog";

            return View(await blogs);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactMe model)
        {
            if (ModelState.IsValid)
            {
                // This is where we will be emailing
                model.Message = $"{model.Message} <hr/> Phone: {model.Phone}";
                await _emailSender.SendContactEmailAsync(model.Email, model.Name, model.Subject, model.Message);
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
