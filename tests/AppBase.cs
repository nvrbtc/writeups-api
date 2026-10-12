using dotnetMVP.Models;
using dotnetMVP.Models.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace NTest
{
    public class AppBase : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private PostgreSqlContainer _pql = null!;
        
        private async Task InitAsync()
        {
            _pql = new PostgreSqlBuilder("postgres:17") // match the version with the one in app
                .WithHostname("localhost")
                .WithDatabase("testdb")
                .WithUsername("testuser")
                .WithPassword("testpassword")
                .Build();
            //init db container 
            await _pql.StartAsync();

            Console.WriteLine();
            //apply all migrations to the db
            using var services = Services.CreateScope();
            var db = services.ServiceProvider.GetRequiredService<ApplicationDBcontext>();
            await db.Database.MigrateAsync();


            //seed the db with testing data
            SeedDatabase seedDatabase = new SeedDatabase(db, services.ServiceProvider.GetRequiredService<UserManager<AppUser>>());
            await seedDatabase.EnsureRolesAndAdmins();
            await seedDatabase.SeedDatabaseDemo();

            services.Dispose();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {

            builder.UseEnvironment("Testing"); // uses appsettings.Testing.json
            base.ConfigureWebHost(builder);


            builder.ConfigureTestServices(config =>
            {
                config.RemoveAll(typeof(DbContextOptions<ApplicationDBcontext>)); // delete the existing configuration for ApplicationDBcontext
                config.RemoveAll(typeof(IDbContextOptionsConfiguration<ApplicationDBcontext>));


                config.AddDbContext<ApplicationDBcontext>(options =>
                {
                    options.UseNpgsql(_pql.GetConnectionString());
                });

            });

        }

        public async Task InitializeAsync()
        {
            await InitAsync();
        }

        public new async Task DisposeAsync()
        {
            await _pql.StopAsync();
        }
    }
}