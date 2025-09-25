using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using PetPal.API.Data;
using Microsoft.AspNetCore.Identity;

namespace PetPal.Tests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Set the environment to Testing
        builder.UseEnvironment("Testing");

        // The Program.cs will now use the in-memory database for the Testing environment

        builder.ConfigureServices(services =>
        {
            // Build the service provider
            var sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<PetPalDbContext>();
            var logger = scopedServices.GetRequiredService<ILogger<TestWebApplicationFactory>>();

            // Configure Identity for testing
            services.Configure<IdentityOptions>(options =>
            {
                // Simplified password requirements for testing
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            });

            try
            {
                // Ensure the database is created
                db.Database.EnsureCreated();

                // Seed the database with test data
                TestHelper.SeedDatabase(scopedServices, db);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the database. Error: {Message}", ex.Message);
            }
        });
    }
}
