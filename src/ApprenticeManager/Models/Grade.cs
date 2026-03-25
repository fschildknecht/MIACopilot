namespace ApprenticeManager.Models;

public class Grade
{
    public int Id { get; set; }
    public int SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;
    public decimal Value { get; set; }
    public DateOnly Date { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
