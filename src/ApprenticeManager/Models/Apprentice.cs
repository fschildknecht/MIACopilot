namespace ApprenticeManager.Models;

public class Apprentice
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string Occupation { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    public ICollection<LearningJournal> LearningJournals { get; set; } = new List<LearningJournal>();

    public string FullName => $"{FirstName} {LastName}";
}
