using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using MvC_SystemMovies.data;
using MvC_SystemMovies.Hubs;
using MvC_SystemMovies.Models;
using MvC_SystemMovies.Repositories;
using MvC_SystemMovies.Repositories.Interfaces;
using MvC_SystemMovies.Services;
using MvC_SystemMovies.Services.Interfaces;

namespace MvC_SystemMovies
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
     options.UseSqlServer(connectionString,
         sqlOptions => sqlOptions.EnableRetryOnFailure(
             maxRetryCount: 5,
             maxRetryDelay: TimeSpan.FromSeconds(10),
             errorNumbersToAdd: null)));


            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            
            builder.Services.AddScoped<IFileStorageService, FileStorageService>();

            
            builder.Services.AddScoped<ICinemaService, CinemaService>();
            builder.Services.AddScoped<IMovieService, MovieService>();

            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IFavoriteService, FavoriteService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IOrderService, OrderService>();

           
            builder.Services.AddSignalR();

            var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowedClients", policy =>
                {
                    if (allowedOrigins.Length > 0)
                    {
                        policy.WithOrigins(allowedOrigins)
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    }
                   
                });
            });

            
            builder.Services
                .AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 6;

                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.AllowedForNewUsers = true;

                    
                    options.SignIn.RequireConfirmedEmail = true;

                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            
            builder.Services.AddRazorPages();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;
            });

            
            builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? string.Empty;
                    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? string.Empty;
                    options.SignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddFacebook(options =>
                {
                    options.AppId = builder.Configuration["Authentication:Facebook:AppId"] ?? string.Empty;
                    options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"] ?? string.Empty;
                    options.SignInScheme = IdentityConstants.ExternalScheme;
                });

            builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
            builder.Services.AddScoped<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IOtpService, OtpService>();

            
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

           
            builder.Services.Configure<MvcOptions>(options =>
            {
                options.Conventions.Add(new AdminAreaAuthorizationConvention());
            });

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

           
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();
                IdentitySeeder.SeedAsync(scope.ServiceProvider).GetAwaiter().GetResult();
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors("AllowedClients");

            app.UseSession();

           
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapRazorPages().WithStaticAssets();

            app.MapHub<NotificationHub>("/hubs/notifications");

            app.MapControllerRoute(
                name: "movieDetails",
                pattern: "Movies/{id:int}/{title?}",
                defaults: new { area = "Customer", controller = "Movie", action = "Details" })
                .WithStaticAssets();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }

  

  
    public class AdminAreaAuthorizationConvention : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        var areaName = controller.RouteValues.TryGetValue("area", out var area) ? area : null;
        if (string.Equals(areaName, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            controller.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter(
                new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireRole("Admin")
                    .Build()));
        }
    }
}
}