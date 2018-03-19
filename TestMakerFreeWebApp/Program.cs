using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TestMakerFreeWebApp.Data;
using TestMakerFreeWebApp.Data.Models;

namespace TestMakerFreeWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);
            using (var scope = host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
        var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();

                dbContext.Database.EnsureCreated();
                DbSeeder.Seed(dbContext, roleManager, userManager);
            }
            host.Run();
        }

        //public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        //    WebHost.CreateDefaultBuilder(args)
        //        .UseStartup<Startup>();

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
