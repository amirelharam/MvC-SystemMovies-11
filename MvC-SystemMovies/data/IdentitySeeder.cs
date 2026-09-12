using Microsoft.AspNetCore.Identity;
using MvC_SystemMovies.Models;

namespace MvC_SystemMovies.data
{
    /// <summary>
    /// Creates the "Admin" and "Customer" roles and a default admin account on
    /// startup so Role/Authorization based features have something to test with.
    /// Called once from Program.cs after the app is built.
    /// </summary>
    public static class IdentitySeeder
    {
        public const string AdminRole = "Admin";
        public const string CustomerRole = "Customer";

        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            foreach (var role in new[] { AdminRole, CustomerRole })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            const string adminEmail = "admin@systemmovies.com";
            const string adminPassword = "Admin@123456";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser is null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Administrator",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, AdminRole);
                }
            }
        }
    }
}
