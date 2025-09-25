using PetPal.API.Models;

namespace PetPal.API.DTOs;

public class UserUpdateDto
{
  public string FirstName { get; set; }
  public string LastName { get; set; }
  public string Email { get; set; }
  public string Phone { get; set; }
  public string Address { get; set; }
}

public class UserUpdateResponseDto
{
  public bool Success { get; set; } = true;
  public string Message { get; set; } = "Profile updated successfully";
  public required UserUpdateDto User { get; set; }
  public DateTime UpdatedAt { get; set; }
}
