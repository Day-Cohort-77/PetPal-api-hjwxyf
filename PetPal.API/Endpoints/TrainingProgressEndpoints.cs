using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetPal.API.Data;
using PetPal.API.DTOs;
using PetPal.API.Models;
using System.Security.Claims;

namespace PetPal.API.Endpoints;

public static class TrainingProgressEndpoints
{
  public static void MapTraingingProgressEndpoints(this WebApplication app)
  {
    // GET training progress
    app.MapGet("/training/{id}", async (
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

      var trainingProgress = await db.TrainingProgresses
        .Include(tp => tp.Pet)
        .ThenInclude(p => p.Owners.Where(o => o.PetId == o.Pet.Id))
        .FirstOrDefaultAsync(t => t.Id == id);

      if (trainingProgress == null)
      {
        return Results.NotFound("training not found.");
      }

      // Check if the user is an admin or owns the pet
      var isAdmin = user.IsInRole("Admin");
      var isPetOwner = trainingProgress.Pet.Owners.Any(po => po.UserProfileId == userProfile.Id);

      if (!isAdmin && !isPetOwner)
      {
        return Results.Forbid();
      }

      return Results.Ok(mapper.Map<TrainingProgressDto>(trainingProgress));
    }).RequireAuthorization();

    // Create a new training
    app.MapPost("/training", async (
        [FromBody] TrainingProgressCreateDto trainingProgressCreateDto,
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

      // Create the training
      var trainingProgress = mapper.Map<TrainingProgress>(trainingProgressCreateDto);
      db.TrainingProgresses.Add(trainingProgress);
      await db.SaveChangesAsync();

      return Results.Created($"/training/{trainingProgress.Id}", mapper.Map<TrainingProgressDto>(trainingProgress));
    }).RequireAuthorization();

    // Update a training
    app.MapPut("/training/{id}", async (
        int id,
        [FromBody] TrainingProgressUpdateDto trainingProgressUpdateDto,
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

      var trainingProgress = await db.TrainingProgresses
        .Include(tp => tp.Pet)
        .ThenInclude(p => p.Owners.Where(o => o.PetId == o.Pet.Id))
        .FirstOrDefaultAsync(t => t.Id == id);

      if (trainingProgress == null)
      {
        return Results.NotFound("Training not found.");
      }

      // Check if the user is an admin or the primary owner of the pet
      var isAdmin = user.IsInRole("Admin");
      var isPetOwner = trainingProgress.Pet.Owners.Any(po => po.UserProfileId == userProfile.Id);

      if (!isAdmin && !isPetOwner)
      {
        return Results.Forbid();
      }

      // Update the pet
      mapper.Map(trainingProgressUpdateDto, trainingProgress);
      trainingProgress.UpdatedAt = DateTime.UtcNow;

      await db.SaveChangesAsync();

      return Results.Ok(mapper.Map<TrainingProgressDto>(trainingProgress));
    }).RequireAuthorization();
  }
}
