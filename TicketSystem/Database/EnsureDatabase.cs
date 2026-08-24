using Microsoft.EntityFrameworkCore;

namespace TicketSystem.Database
{
    public static class EnsureDatabase
    {
        public static void Migrate(IApplicationBuilder app)
        {
            TicketSystemDbContext ctx = app.ApplicationServices
                                            .CreateScope()
                                            .ServiceProvider
                                            .GetRequiredService<TicketSystemDbContext>();

            if (ctx.Database.GetPendingMigrations().Any())
            {
                ctx.Database.Migrate();
            }
        }
        //public static void SeedDatabase(IApplicationBuilder app)
        //{
        //    KundeProjektDbContext ctx = app.ApplicationServices
        //                                    .CreateScope()
        //                                    .ServiceProvider
        //                                    .GetRequiredService<KundeProjektDbContext>();
        //    if (!ctx.Customers.Any())
        //    {
        //        ctx.Customers.AddRange(
        //            new Customer() { Address = "Heldenstraße 1", Company = "Hero Company" },
        //            new Customer() { Address = "Unterweltsstraße 66", Company = "Hades Se" },
        //            new Customer() { Address = "Seestraße 15", Company = "Aqua Seeds lt" });
        //        ctx.SaveChanges();
        //    }
        //}
    }
}
