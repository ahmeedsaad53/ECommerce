
using E_Commerce.Date;
using Microsoft.AspNetCore.Identity;
namespace ECommerce.Date
{
    public static class DbInitializer
    {

        public static async Task SeedRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new IdentityRole("User"));
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var admin = await userManager.FindByEmailAsync("saad@gmail.com");

            if (admin == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = "saaad",
                    Email = "saad@gmail.com"//email
                };
                var result = await userManager.CreateAsync(newAdmin, "Ahmed*1saad");//pasword

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
        }
    
    }
}
