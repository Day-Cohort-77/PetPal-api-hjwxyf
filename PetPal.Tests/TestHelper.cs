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
        await SeedPetOwners(dbContext);
        SeedMedications(dbContext);

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

    private static async Task SeedPetOwners(PetPalDbContext context)
    {

        var petOwner1 = new PetOwner
        {
            PetId = 1,
            UserProfileId = 1,
            IsPrimaryOwner = true
        };
        var petOwner2 = new PetOwner
        {
            PetId = 2,
            UserProfileId = 2,
            IsPrimaryOwner = true
        };
        var petOwner3 = new PetOwner
        {
            PetId = 3,
            UserProfileId = 3,
            IsPrimaryOwner = false
        };

        context.PetOwners.AddRange(petOwner1, petOwner2, petOwner3);
        await context.SaveChangesAsync();
    }

    public static void SeedTraining(PetPalDbContext dbContext)
    {

        var training1 = new TrainingProgress
        {
            Id = 1,
            Name = "TrainingName1",
            Goals = ["Sit", "Beg"],
            Hours = 2,
            PetId = 1
        };
        var training2 = new TrainingProgress
        {
            Id = 2,
            Name = "TrainingName2",
            Goals = ["Roll-over"],
            Hours = 5,
            PetId = 2
        };
        var training3 = new TrainingProgress
        {
            Id = 3,
            Name = "TrainingName3",
            Goals = ["Don't poop on things", "Don't pee on things", "Stop Tearing up the couch"],
            Hours = 42,
            PetId = 3
        };

        dbContext.TrainingProgresses.AddRange(training1, training2, training3);
        dbContext.SaveChanges();

    }
    public static async Task<List<PetDto>?> GetAllPetsAsync(HttpClient client)
    {
        var response = await client.GetAsync("/pets");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<PetDto>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public static async Task<UserUpdateResponseDto?> UpdateUserProfileAsync(HttpClient client, UserUpdateDto userUpdateDto, int id)
    {
        var response = await client.PutAsJsonAsync($"/users/{id}", userUpdateDto);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<UserUpdateResponseDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public static async Task<List<TrainingProgressDto>?> GetAllTrainingsAsync(HttpClient client, int petId)
    {
        var response = await client.GetAsync($"/pets/{petId}/trainings");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<TrainingProgressDto>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public static async Task<TrainingProgressDto?> CreateTrainingProgressAsync(HttpClient client, TrainingProgressCreateDto trainingProgressCreateDto)
    {
        var response = await client.PostAsJsonAsync($"/training", trainingProgressCreateDto);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TrainingProgressDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public static async Task<TrainingProgressDto?> UpdateTrainingProgressAsync(HttpClient client, TrainingProgressUpdateDto trainingProgressUpdateDto, int id)
    {
        var response = await client.PutAsJsonAsync($"/training/{id}", trainingProgressUpdateDto);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TrainingProgressDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public static async Task<List<MedicationDto>> GetPetMedicationsAsync(HttpClient client, int petId)
    {
        var response = await client.GetAsync($"/pets/{petId}/medications");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<MedicationDto>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public static async Task<MedicationDto> CreateMedicationAsync(HttpClient client, MedicationCreateDto medicationDto)
    {
        var response = await client.PostAsJsonAsync($"/medications", medicationDto);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<MedicationDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    private static void SeedMedications(PetPalDbContext dbContext)
    {
        var medications = new List<Medication>
        {
            new Medication
            {
                Name = "Amoxicillin",
                PetId = 1,
                Dosage = "50mg",
                Frequency = "Twice daily",
                StartDate = DateTime.UtcNow.AddDays(-10),
                EndDate = DateTime.UtcNow.AddDays(4),
                Instructions = "Give with food",
                PrescribedBy = new PrescribedBy { Id = "vet1", Name = "Dr. Smith" },
                Status = "active",
                ReminderEnabled = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddDays(-10)
            },
            new Medication
            {
                Name = "Carprofen",
                PetId = 1,
                Dosage = "25mg",
                Frequency = "Once daily",
                StartDate = DateTime.UtcNow.AddDays(-5),
                EndDate = DateTime.UtcNow.AddDays(10),
                Instructions = "Give with meals to reduce stomach upset",
                PrescribedBy = new PrescribedBy { Id = "vet2", Name = "Dr. Johnson" },
                Status = "active",
                ReminderEnabled = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new Medication
            {
                Name = "Prednisone",
                PetId = 2,
                Dosage = "10mg",
                Frequency = "Once daily",
                StartDate = DateTime.UtcNow.AddDays(-15),
                EndDate = DateTime.UtcNow.AddDays(-2),
                Instructions = "Taper dosage as directed",
                PrescribedBy = new PrescribedBy { Id = "vet3", Name = "Dr. Wilson" },
                Status = "completed",
                ReminderEnabled = false,
                IsActive = false,
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                UpdatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new Medication
            {
                Name = "Metronidazole",
                PetId = 2,
                Dosage = "100mg",
                Frequency = "Twice daily",
                StartDate = DateTime.UtcNow.AddDays(-3),
                EndDate = DateTime.UtcNow.AddDays(4),
                Instructions = "Complete full course even if symptoms improve",
                PrescribedBy = new PrescribedBy { Id = "vet4", Name = "Dr. Brown" },
                Status = "active",
                ReminderEnabled = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                UpdatedAt = DateTime.UtcNow.AddDays(-3)
            },

        };

        dbContext.Medications.AddRange(medications);
        dbContext.SaveChanges();
    }

}
