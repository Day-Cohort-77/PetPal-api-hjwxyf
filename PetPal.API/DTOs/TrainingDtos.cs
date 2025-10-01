public class TrainingProgressDto
{
  public int Id { get; set; }
  public string Name { get; set; }
  public List<string>? Goals { get; set; }
  public int Hours { get; set; }
  public int PetId { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
}

public class TrainingProgressCreateDto
{
  public string Name { get; set; }
  public List<string>? Goals { get; set; }
  public int PetId { get; set; }
}

public class TrainingProgressUpdateDto
{
  public List<string>? Goals { get; set; }
  public int Hours { get; set; }
}
