using Microsoft.AspNetCore.Identity;

namespace TicketSystem.Database
{
    public static class EnsureIdentity
    {
        // Rollen
        private const string adminRole = "Admin";
        private const string devRole = "Developer";
        private const string testRole = "Tester";

        // User
        private const string adminName = "Admin";
        private const string adminPW = "Secret123$";

        private const string devName = "Dev";
        private const string devPW = "Secret123$";

        private const string testName = "Tester";
        private const string testPW = "Secret123$";

        public static async void SeedDefaultAccounts(IApplicationBuilder app)
        {
            RoleManager<IdentityRole> roleManager = app.ApplicationServices
                               .CreateScope()
                               .ServiceProvider
                               .GetRequiredService<RoleManager<IdentityRole>>();
            UserManager<IdentityUser> userManager = app.ApplicationServices
                                          .CreateScope()
                                          .ServiceProvider
                                          .GetRequiredService<UserManager<IdentityUser>>();
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                IdentityRole role = new IdentityRole(adminRole);
                await roleManager.CreateAsync(role);
            }
            if (!await roleManager.RoleExistsAsync(devRole))
            {
                IdentityRole role = new IdentityRole(devRole);
                await roleManager.CreateAsync(role);
            }
            if (!await roleManager.RoleExistsAsync(testRole))
            {
                IdentityRole role = new IdentityRole(testRole);
                await roleManager.CreateAsync(role);
            }

            IdentityUser? admin = await userManager.FindByNameAsync(adminName);
            if (admin == null)
            {
                admin = new IdentityUser(adminName);
                await userManager.CreateAsync(admin, adminPW);
                await userManager.AddToRoleAsync(admin, adminRole);

            }
            IdentityUser? dev = await userManager.FindByNameAsync(devName);
            if (dev == null)
            {
                dev = new IdentityUser(devName);
                await userManager.CreateAsync(dev, devPW);
                await userManager.AddToRoleAsync(dev, devRole);
            }
            IdentityUser? tester = await userManager.FindByNameAsync(testName);
            if (tester == null)
            {
                tester = new IdentityUser(testName);
                await userManager.CreateAsync(tester, testPW);
                await userManager.AddToRoleAsync(tester, testRole);
            }
        }
    }
}
