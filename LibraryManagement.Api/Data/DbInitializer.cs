using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Api.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {

            var context = services.GetRequiredService<LibraryDbContext>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            try
            {
                if (context.Database.GetPendingMigrations().Any())
                {
                   await context.Database.MigrateAsync();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
          
            string[] roles = { "Administrator", "Librarian", "Staff" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = "admin@library.com";

            var admin = await userManager.FindByNameAsync(adminEmail);

            if (admin == null)
            {
                admin = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail  ,
                    PhoneNumber = "01005415303",
                };

                var result = await userManager.CreateAsync(admin, "Admin@123");
                Console.WriteLine( string.Join(  '\n',result.Errors.Select(e => e.Description)) );

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Administrator");
                }
            } 
    
        }
    }
}
