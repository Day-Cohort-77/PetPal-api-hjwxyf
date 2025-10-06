namespace PetPal.API.Models;

public class TrainingProgress
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<string>? Goals { get; set; }
    public int Hours { get; set; }
    public int PetId { get; set; }
    public Pet Pet { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
