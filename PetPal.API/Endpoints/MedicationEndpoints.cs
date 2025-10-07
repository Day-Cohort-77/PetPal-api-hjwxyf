using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetPal.API.Data;
using PetPal.API.DTOs;
using PetPal.API.Models;
using System.Security.Claims;

namespace PetPal.API.Endpoints;

public static class MedicationEndpoints
{
    public static void MapMedicationEndpoints(this WebApplication app)
    {
        // Get all medications for a pet
        app.MapGet("/pets/{petId}/medications", async (
            int petId,
            ClaimsPrincipal user,
            PetPalDbContext db,
            IMapper mapper) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var userProfile = await db.UserProfiles
                .FirstOrDefaultAsync(up => up.IdentityUserId == userId);

            if (userProfile == null)
            {
                return Results.NotFound("User profile not found.");
            }

            var pet = await db.Pets
                .Include(p => p.Owners)
                .FirstOrDefaultAsync(p => p.Id == petId);

            if (pet == null)
            {
                return Results.NotFound("Pet not found.");
            }

            // Check if the user is an admin, vet, or owns the pet
            var isAdmin = user.IsInRole("Admin");
            var isVet = user.IsInRole("Veterinarian");
            var isPetOwner = pet.Owners.Any(po => po.UserProfileId == userProfile.Id);

            if (!isAdmin && !isVet && !isPetOwner)
            {
                return Results.Forbid();
            }

            var medications = await db.Medications
                .Include(m => m.Pet)
                .Where(m => m.PetId == petId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            return Results.Ok(mapper.Map<List<MedicationDto>>(medications));
        }).RequireAuthorization();

        // Get a specific medication
        app.MapGet("/medications/{id}", async (
            int id,
            ClaimsPrincipal user,
            PetPalDbContext db,
            IMapper mapper) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var userProfile = await db.UserProfiles
                .FirstOrDefaultAsync(up => up.IdentityUserId == userId);

            if (userProfile == null)
            {
                return Results.NotFound("User profile not found.");
            }

            var medication = await db.Medications
                .Include(m => m.Pet)
                .ThenInclude(p => p.Owners)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medication == null)
            {
                return Results.NotFound("Medication not found.");
            }

            // Check if the user is an admin, vet, or owns the pet
            var isAdmin = user.IsInRole("Admin");
            var isVet = user.IsInRole("Veterinarian");
            var isPetOwner = medication.Pet.Owners.Any(po => po.UserProfileId == userProfile.Id);

            if (!isAdmin && !isVet && !isPetOwner)
            {
                return Results.Forbid();
            }

            return Results.Ok(mapper.Map<MedicationDto>(medication));
        }).RequireAuthorization();

        // Create a new medication
        app.MapPost("/medications", async (
            [FromBody] MedicationCreateDto medicationDto,
            ClaimsPrincipal user,
            PetPalDbContext db,
            IMapper mapper) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var userProfile = await db.UserProfiles
                .FirstOrDefaultAsync(up => up.IdentityUserId == userId);

            if (userProfile == null)
            {
                return Results.NotFound("User profile not found.");
            }

            var pet = await db.Pets
                .Include(p => p.Owners)
                .FirstOrDefaultAsync(p => p.Id == medicationDto.PetId);

            if (pet == null)
            {
                return Results.NotFound("Pet not found.");
            }

            // Check if the user is an admin, vet, or the primary owner of the pet
            var isAdmin = user.IsInRole("Admin");
            var isVet = user.IsInRole("Veterinarian");
            var isPrimaryOwner = pet.Owners.Any(po => po.UserProfileId == userProfile.Id && po.IsPrimaryOwner);

            if (!isAdmin && !isVet && !isPrimaryOwner)
            {
                return Results.Forbid();
            }

            // Create the medication
            var medication = mapper.Map<Medication>(medicationDto);
            db.Medications.Add(medication);
            await db.SaveChangesAsync();

            // Reload the medication with related entities
            var reloadedMedication = await db.Medications
                .Include(m => m.Pet)
                .FirstOrDefaultAsync(m => m.Id == medication.Id);

            return Results.Created($"/medications/{medication.Id}", mapper.Map<MedicationDto>(reloadedMedication));
        }).RequireAuthorization();

        // Update a medication
        app.MapPut("/medications/{id}", async (
            int id,
            [FromBody] MedicationUpdateDto medicationDto,
            ClaimsPrincipal user,
            PetPalDbContext db,
            IMapper mapper) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var userProfile = await db.UserProfiles
                .FirstOrDefaultAsync(up => up.IdentityUserId == userId);

            if (userProfile == null)
            {
                return Results.NotFound("User profile not found.");
            }

            var medication = await db.Medications
                .Include(m => m.Pet)
                .ThenInclude(p => p.Owners)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medication == null)
            {
                return Results.NotFound("Medication not found.");
            }

            // Check if the user is an admin, vet, or the primary owner of the pet
            var isAdmin = user.IsInRole("Admin");
            var isVet = user.IsInRole("Veterinarian");
            var isPrimaryOwner = medication.Pet.Owners.Any(po => po.UserProfileId == userProfile.Id && po.IsPrimaryOwner);

            if (!isAdmin && !isVet && !isPrimaryOwner)
            {
                return Results.Forbid();
            }

            // Update the medication
            mapper.Map(medicationDto, medication);
            medication.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            // Reload the medication with related entities
            medication = await db.Medications
                .Include(m => m.Pet)
                .FirstOrDefaultAsync(m => m.Id == medication.Id);

            return Results.Ok(mapper.Map<MedicationDto>(medication));
        }).RequireAuthorization();

        // Delete a medication
        app.MapDelete("/medications/{id}", async (
            int id,
            ClaimsPrincipal user,
            PetPalDbContext db) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var userProfile = await db.UserProfiles
                .FirstOrDefaultAsync(up => up.IdentityUserId == userId);

            if (userProfile == null)
            {
                return Results.NotFound("User profile not found.");
            }

            var medication = await db.Medications
                .Include(m => m.Pet)
                .ThenInclude(p => p.Owners)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medication == null)
            {
                return Results.NotFound("Medication not found.");
            }

            // Check if the user is an admin, vet, or the primary owner of the pet
            var isAdmin = user.IsInRole("Admin");
            var isVet = user.IsInRole("Veterinarian");
            var isPrimaryOwner = medication.Pet.Owners.Any(po => po.UserProfileId == userProfile.Id && po.IsPrimaryOwner);

            if (!isAdmin && !isVet && !isPrimaryOwner)
            {
                return Results.Forbid();
            }

            // Delete the medication
            db.Medications.Remove(medication);
            await db.SaveChangesAsync();

            return Results.NoContent();
        }).RequireAuthorization();
    }
}