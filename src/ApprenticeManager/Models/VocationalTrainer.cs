namespace ApprenticeManager.Models;

/// <summary>Represents a vocational trainer (Lehrlingsbetreuer) who oversees apprentices.</summary>
public class VocationalTrainer
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }

    /// <summary>Returns the full name of the trainer.</summary>
    public string FullName => $"{FirstName} {LastName}";
}
