namespace ApprenticeManager.Models;

public class Subject
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ApprenticeId { get; set; }
    public Apprentice Apprentice { get; set; } = null!;
    public ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public double? AverageGrade => Grades.Any()
        ? Math.Round(Grades.Average(g => (double)g.Value), 2)
        : null;
}
