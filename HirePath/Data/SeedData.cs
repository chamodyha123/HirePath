using HirePathAI.API.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace HirePathAI.API.Data
{
    public static class SeedData
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            string[] roles = { "Admin", "Candidate", "Recruiter", "HiringManager" };

            // Create roles if they do not exist
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>
                    {
                        Name = role
                    });
                }
            }

            // Create default admin user if not exists
            var adminEmail = "admin@hirepath.com";
            var adminUserName = "admin";
            var adminPassword = "Admin@123";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new User
                {
                    FullName = "System Admin",
                    UserName = adminUserName,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}