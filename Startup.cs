using BlogProject.Data;
using BlogProject.Models;
using BlogProject.Services;
using BlogProject.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Reflection;

namespace BlogProject
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(ConnectionService.GetConnectionString(Configuration)));

            services.AddDatabaseDeveloperPageExceptionFilter();

            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddIdentity<BlogUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddDefaultUI()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddControllersWithViews();
            services.AddRazorPages();
            
            // Swagger API
            services.AddSwaggerGen(options =>
            {
                // using System.Reflection;
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("https://riosr.com",
                                                    "https://*.riosr.com")
                                .SetIsOriginAllowedToAllowWildcardSubdomains();
                        // Allow localhost during development and testing
                        //builder.SetIsOriginAllowed(origin => new Uri(origin).IsLoopback);
                    });
            });

            // Register my custom DataService class
            services.AddScoped<DataService>();

            // Register my preconfigured instance of the MailSettings class
            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));

            // Register my custom IBlogEmailSender interface and implementation
            services.AddScoped<IBlogEmailSender, EmailService>();

            // Register my Image Service
            services.AddScoped<IImageService, BasicImageService>();

            // Register my Slug Service
            services.AddScoped<ISlugService, BasicSlugService>();

            // Register my Blog Search Service
            services.AddScoped<BlogSearchService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();

                // Swagger API
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "SlugRoute",
                    pattern: "BlogPosts/UrlFriendly/{slug}",
                    defaults: new { controller = "Posts", action = "Details" });
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
