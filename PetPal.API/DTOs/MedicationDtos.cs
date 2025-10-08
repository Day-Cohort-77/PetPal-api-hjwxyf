namespace PetPal.API.DTOs;

public class MedicationDto
{
    public int Id { get; set; }
    public int PetId { get; set; }
    public string PetName { get; set; }
    public string Name { get; set; }
    public string Dosage { get; set; }
    public string Frequency { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Instructions { get; set; }
    public PrescribedByDto PrescribedBy { get; set; }
    public string Status { get; set; } = "active";
    public bool ReminderEnabled { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class MedicationCreateDto
{
    public int PetId { get; set; }
    public string Name { get; set; }
    public string Dosage { get; set; }
    public string Frequency { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Instructions { get; set; }
    public PrescribedByDto PrescribedBy { get; set; }
    public string Status { get; set; } = "active";
    public bool ReminderEnabled { get; set; } = true;
}

public class MedicationUpdateDto
{
    public string Name { get; set; }
    public string Dosage { get; set; }
    public string Frequency { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Instructions { get; set; }
    public PrescribedByDto PrescribedBy { get; set; }
    public string Status { get; set; }
    public bool ReminderEnabled { get; set; }
    public bool IsActive { get; set; }
}

public class PrescribedByDto
{
    public string Id { get; set; }
    public string Name { get; set; }
}