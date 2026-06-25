using Microsoft.AspNetCore.Identity;
using RecyclingWorld.Models;

namespace RecyclingWorld.Data
{
    
        public static class DbInitializer
        {
            public static async Task SeedRolesAndAdminAsync(IServiceProvider services)
            {
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

                string[] roles = { "Admin", "Supplier", "Buyer" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                string adminEmail = "admin@admin.com";
                if (await userManager.FindByEmailAsync(adminEmail) == null)
                {
                    var admin = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true,
                        Name = "Site Admin",
                        StreetAddress = "1 Way or the Highway",
                        City = "Wellington",
                        State = "Wellington",
                        PostalCode = "6011",
                        Country = "New Zealand"
                    };

                    var result = await userManager.CreateAsync(admin, "Admin1!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "Admin");
                    }

                }

            }
        }
    }
