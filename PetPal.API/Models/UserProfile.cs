using Microsoft.AspNetCore.Identity;

namespace PetPal.API.Models;

public class UserProfile
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public string IdentityUserId { get; set; }
    public IdentityUser IdentityUser { get; set; }
    public List<PetOwner> OwnedPets { get; set; } = new List<PetOwner>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    

     public string Theme { get; set; } = "light";
    public string ColorAccent { get; set; } = "#4a90e2";
    public string FontSize { get; set; } = "medium";
    public bool UseSystemPreference { get; set; } = false;
}