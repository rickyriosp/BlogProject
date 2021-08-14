using BlogProject.Data;
using BlogProject.Enums;
using BlogProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        public DataService(ApplicationDbContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<BlogUser> userManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
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
            // IF there are already Users in the system: do nothing
            if (_dbContext.Users.Any())
            {
                return;
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
                Email = "ricky@riosr.com",
                UserName = "ricky@riosr.com",
                FirstName = "Ricky",
                LastName = "Rios",
                DisplayName = "RickyMod",
                PhoneNumber = "(407) 548-4990",
                EmailConfirmed = true,
            };

            // Step 2 Repeat: Interact with the User Manager to create a new User defined by modUser
            // IdentityUser Password needs UpperCase, Number, and Symbol
            createdUser = await _userManager.CreateAsync(modUser, "Password123!");

            // Step 3 Repeat: Add the new User to the Moderator role
            if (await _roleManager.RoleExistsAsync(BlogRole.Moderator.ToString()) && createdUser.Succeeded)
            {
                await _userManager.AddToRoleAsync(modUser, BlogRole.Moderator.ToString());
            }
        }
    }
}
