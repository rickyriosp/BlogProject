using BlogProject.Data;
using BlogProject.Enums;
using BlogProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogProject.Services
{
    public class DataService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BlogUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IImageService _imageService;

        public DataService(ApplicationDbContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<BlogUser> userManager, IConfiguration configuration, IImageService imageService)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
            _imageService = imageService;
        }

        public async Task ManageDataAsync()
        {
            // Task 1: Create DB from the Migrations
            await _dbContext.Database.MigrateAsync();

            // Task 2: Seed a few Roles into the system
            await SeedRolesAsync();

            // Task 3: Seed a few Users into the system
            await SeedUsersAsync();
        }

        private async Task SeedRolesAsync()
        {
            // If there are already Roles in the system: do nothing
            if (_dbContext.Roles.Any())
            {
                return;
            }

            // Otherwise we want to create a few Roles
            foreach (var role in Enum.GetNames(typeof(BlogRole)))
            {
                // Interact with the Role Manager to create Roles
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        private async Task SeedUsersAsync()
        {
            // If there are already Users in the system: do nothing
            if (_dbContext.Users.Any())
            {
                //return;
            }

            // Otherwise we want to create a few Users

            // Step 1: Create a new instance of BlogUser
            var adminUser = new BlogUser()
            {
                Email = "ricardo@riosr.com",
                UserName = "ricardo@riosr.com",
                FirstName = "Ricky",
                LastName = "Rios",
                DisplayName = "RickyAdmin",
                PhoneNumber = "(407) 548-4990",
                EmailConfirmed = true,
                ImageData = await _imageService.EncodeImageAsync(_configuration["DefaultUserImage"]),
                ContentType = ".png",
            };

            // Step 2: Interact with the User Manager to create a new User defined by adminUser
            // IdentityUser Password needs UpperCase, Number, and Symbol
            var createdUser = await _userManager.CreateAsync(adminUser, "Password123!");
            
            // Step 3: Add the new User to the Administrator role
            if (await _roleManager.RoleExistsAsync(BlogRole.Administrator.ToString()) && createdUser.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, BlogRole.Administrator.ToString());
            }

            // Step 1 Repeat: Create a new instance of BlogUser
            var modUser = new BlogUser()
            {
                Email = "riki.rios91@gmail.com",
                UserName = "riki.rios91@gmail.com",
                FirstName = "Ricky",
                LastName = "Rios",
                DisplayName = "RickyMod",
                PhoneNumber = "(407) 548-4990",
                EmailConfirmed = true,
                ImageData = await _imageService.EncodeImageAsync(_configuration["DefaultUserImage"]),
                ContentType = ".png",
            };

            // Step 2 Repeat: Interact with the User Manager to create a new User defined by modUser
            // IdentityUser Password needs UpperCase, Number, and Symbol
            createdUser = await _userManager.CreateAsync(modUser, "Password123!");

            // Step 3 Repeat: Add the new User to the Moderator role
            if (await _roleManager.RoleExistsAsync(BlogRole.Moderator.ToString()) && createdUser.Succeeded)
            {
                await _userManager.AddToRoleAsync(modUser, BlogRole.Moderator.ToString());
            }

            // Step 1 Repeat: Demo User
            var demoUser = new BlogUser()
            {
                Email = "demo.user@mailinator.com",
                UserName = "demo.user@mailinator.com",
                FirstName = "Demo",
                LastName = "User",
                DisplayName = "DemoUser",
                PhoneNumber = "(123) 456-7890",
                EmailConfirmed = true,
                ImageData = await _imageService.EncodeImageAsync(_configuration["DefaultUserImage"]),
                ContentType = ".png",
            };

            // Step 2 Repeat: Interact with the User Manager to create a new User defined by modUser
            // IdentityUser Password needs UpperCase, Number, and Symbol
            createdUser = await _userManager.CreateAsync(demoUser, "Password123!");

            // Step 3 Repeat: Add the new User to the Moderator role
            if (await _roleManager.RoleExistsAsync(BlogRole.User.ToString()) && createdUser.Succeeded)
            {
                await _userManager.AddToRoleAsync(demoUser, BlogRole.User.ToString());
            }

            // Step 1 Repeat: Demo Moderator
            var demoMod = new BlogUser()
            {
                Email = "demo.moderator@mailinator.com",
                UserName = "demo.moderator@mailinator.com",
                FirstName = "Demo",
                LastName = "Moderator",
                DisplayName = "DemoModerator",
                PhoneNumber = "(123) 456-7890",
                EmailConfirmed = true,
                ImageData = await _imageService.EncodeImageAsync(_configuration["DefaultUserImage"]),
                ContentType = ".png",
            };

            // Step 2 Repeat: Interact with the User Manager to create a new User defined by modUser
            // IdentityUser Password needs UpperCase, Number, and Symbol
            createdUser = await _userManager.CreateAsync(demoMod, "Password123!");

            // Step 3 Repeat: Add the new User to the Moderator role
            if (await _roleManager.RoleExistsAsync(BlogRole.Moderator.ToString()) && createdUser.Succeeded)
            {
                await _userManager.AddToRoleAsync(demoMod, BlogRole.Moderator.ToString());
            }

            // Step 1 Repeat: Demo Admin
            var demoAdmin = new BlogUser()
            {
                Email = "demo.admin@mailinator.com",
                UserName = "demo.admin@mailinator.com",
                FirstName = "Demo",
                LastName = "Admin",
                DisplayName = "DemoAdmin",
                PhoneNumber = "(123) 456-7890",
                EmailConfirmed = true,
                ImageData = await _imageService.EncodeImageAsync(_configuration["DefaultUserImage"]),
                ContentType = ".png",
            };

            // Step 2 Repeat: Interact with the User Manager to create a new User defined by modUser
            // IdentityUser Password needs UpperCase, Number, and Symbol
            createdUser = await _userManager.CreateAsync(demoAdmin, "Password123!");

            // Step 3 Repeat: Add the new User to the Moderator role
            if (await _roleManager.RoleExistsAsync(BlogRole.Administrator.ToString()) && createdUser.Succeeded)
            {
                await _userManager.AddToRoleAsync(demoAdmin, BlogRole.Administrator.ToString());
            }
        }
    }
}
