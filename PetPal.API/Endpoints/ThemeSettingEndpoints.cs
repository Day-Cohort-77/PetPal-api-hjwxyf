using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PetPal.API.Data;
using PetPal.API.Models;
using System.Security.Claims;

namespace PetPal.API.Endpoints;

public static class ThemeSettingEndpoints
{
    public static void MapThemeSettingEndpoints(this WebApplication app)
    {   //get user theme preferences
        app.MapGet("/api/theme-settings", async (
       ClaimsPrincipal user,
       PetPalDbContext db) =>
   {
       var currentUserId = user.FindFirstValue(ClaimTypes.NameIdentifier);
       if (currentUserId == null)
       {
           return Results.Unauthorized();
       }

       var userProfile = await db.UserProfiles
           .FirstOrDefaultAsync(up => up.IdentityUserId == currentUserId);

       if (userProfile == null)
       {
           return Results.NotFound("User profile not found.");
       }

      
       return Results.Ok(new
       {
           success = true,
           message = "Theme preferences retrieved successfully",
           preferences = new
           {
               userId = currentUserId,
               theme = userProfile.Theme, // Default to light if null
               colorAccent = userProfile.ColorAccent,
               fontSize = userProfile.FontSize,
               useSystemPreference = userProfile.UseSystemPreference,
               updatedAt = userProfile.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
           }
       });
   }).RequireAuthorization();

        // save user theme preferences
        app.MapPost("/api/theme-settings", async (
            ClaimsPrincipal user,
            PetPalDbContext db,
            ThemeRequest request) =>
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

           
            userProfile.Theme = request.Theme ?? "light";
            userProfile.ColorAccent = request.ColorAccent ?? "#4a90e2";
            userProfile.FontSize = request.FontSize ?? "medium";
            userProfile.UseSystemPreference = request.UseSystemPreference;
            userProfile.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();


            return Results.Ok(new
            {
                success = true,
                message = "Theme preferences updated successfully",
                preferences = new
                {
                    userId = userId,
                    theme = userProfile.Theme,
                    colorAccent = userProfile.ColorAccent,
                    fontSize = userProfile.FontSize,
                    useSystemPreference = userProfile.UseSystemPreference,
                    updatedAt = userProfile.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
                }
            });
        }).RequireAuthorization();
        // update user theme preferences
        app.MapPut("/api/theme-settings", async (
    ClaimsPrincipal user,
    PetPalDbContext db,
    ThemeRequest request) =>
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

    // Update the theme properties
    userProfile.Theme = request.Theme ?? userProfile.Theme;
    userProfile.ColorAccent = request.ColorAccent ?? userProfile.ColorAccent;
    userProfile.FontSize = request.FontSize ?? userProfile.FontSize;
    userProfile.UseSystemPreference = request.UseSystemPreference;
    userProfile.UpdatedAt = DateTime.UtcNow;

    await db.SaveChangesAsync();

    return Results.Ok(new
    {
        success = true,
        message = "Theme preferences updated successfully",
        preferences = new
        {
            userId = userId,
            theme = userProfile.Theme,
            colorAccent = userProfile.ColorAccent,
            fontSize = userProfile.FontSize,
            useSystemPreference = userProfile.UseSystemPreference,
            updatedAt = userProfile.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
        }
    });
}).RequireAuthorization();
        // reset user theme preferences to default
app.MapDelete("/api/theme-settings", async (
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

    //default values
    userProfile.Theme = "light";            
    userProfile.ColorAccent = "#4a90e2";   
    userProfile.FontSize = "medium";         
    userProfile.UseSystemPreference = false; 
    
    await db.SaveChangesAsync();

     return Results.Ok(new
    {
        success = true,
        message = "Theme preferences reset to defaults",
        preferences = new
        {
            theme = userProfile.Theme,
            colorAccent = userProfile.ColorAccent,
            fontSize = userProfile.FontSize,
            useSystemPreference = userProfile.UseSystemPreference
        }
    });
}).RequireAuthorization();
    }
}
       