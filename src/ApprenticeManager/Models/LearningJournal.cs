namespace ApprenticeManager.Models;

public class LearningJournal
{
    public int Id { get; set; }
    public int ApprenticeId { get; set; }
    public Apprentice Apprentice { get; set; } = null!;
    public int Kalenderwoche { get; set; }
    public int Jahr { get; set; }
    public string BetrieblicheTaetigkeiten { get; set; } = string.Empty;
    public string SchulischeTaetigkeiten { get; set; } = string.Empty;
    public string Reflexion { get; set; } = string.Empty;
    public DateTime ErstelltAm { get; set; }
    public DateTime? GeaendertAm { get; set; }

    public string Titel => $"KW {Kalenderwoche}/{Jahr}";
}
