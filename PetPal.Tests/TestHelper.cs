using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Engine.ClientProtocol;
using PetPal.API.Data;
using PetPal.API.DTOs;
using PetPal.API.Models;
using Xunit.Abstractions;

namespace PetPal.Tests;

public static class TestHelper
{
    public static async Task SeedDatabase(IServiceProvider serviceProvider, PetPalDbContext dbContext)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Seed roles
        await SeedRoles(roleManager);

        // Seed admin user
        await SeedAdminUser(userManager, dbContext);

        // Seed sample data
        SeedPets(dbContext);

    }

    private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "Admin", "User", "Veterinarian" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task SeedAdminUser(UserManager<IdentityUser> userManager, PetPalDbContext context)
    {
        var adminEmail = "admin@petpal.com";

        var adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(adminUser, "Admin123!");

        await userManager.AddToRoleAsync(adminUser, "Admin");

        var adminProfile = new UserProfile
        {
            FirstName = "Admin",
            LastName = "User",
            Email = adminEmail,
            Address = "123 Admin St",
            Phone = "555-123-4567",
            IdentityUserId = adminUser.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,

            //theme settings will use default values
            Theme = "light",
            ColorAccent = "#4a90e2",
            FontSize = "medium",
            UseSystemPreference = false
        };

        context.UserProfiles.Add(adminProfile);
        await context.SaveChangesAsync();

        // Verify the admin profile was created
        var verifyProfile = await context.UserProfiles.FirstOrDefaultAsync(up => up.IdentityUserId == adminUser.Id);
        if (verifyProfile == null)
        {
            // If profile creation failed, log it and try again
            Console.WriteLine("WARNING: Admin profile creation failed on first attempt. Trying again...");

            // Try creating the profile again
            context.UserProfiles.Add(adminProfile);
            await context.SaveChangesAsync();
        }
    }
    public static void SeedPets(PetPalDbContext dbContext)
    {

        var pet1 = new Pet
        {
            Id = 1,
            Name = "Name1",
            Species = "TestSpecies1",
            Breed = "TestBreed1",
            Color = "Blue",
            DateOfBirth = new DateTime(2020, 12, 25, 10, 30, 50),
            ImageUrl = "image1@example.com",
            MicrochipNumber = "111"
        };
        var pet2 = new Pet
        {
            Id = 2,
            Name = "Name2",
            Species = "TestSpecies2",
            Breed = "TestBreed2",
            Color = "Red",
            DateOfBirth = new DateTime(2022, 12, 25, 10, 30, 50),
            ImageUrl = "image2@example.com",
            MicrochipNumber = "222"
        };
        var pet3 = new Pet
        {
            Id = 3,
            Name = "Name3",
            Species = "TestSpecies3",
            Breed = "TestBreed3",
            Color = "Green",
            DateOfBirth = new DateTime(2021, 12, 25, 10, 30, 50),
            ImageUrl = "image3@example.com",
            MicrochipNumber = "333"
        };

        dbContext.Pets.AddRange(pet1, pet2, pet3);
        dbContext.SaveChanges();

    }
    public static async Task<List<PetDto>> GetAllPetsAsync(HttpClient client)
    {
        var response = await client.GetAsync("/pets");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<PetDto>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
    
    public static async Task<UserUpdateResponseDto> UpdateUserProfileAsync(HttpClient client, UserUpdateDto userUpdateDto, int id)
    {
        var response = await client.PutAsJsonAsync($"/users/{id}", userUpdateDto);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<UserUpdateResponseDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

}
