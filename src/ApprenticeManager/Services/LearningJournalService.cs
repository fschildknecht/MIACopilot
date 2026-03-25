using ApprenticeManager.Data;
using ApprenticeManager.Models;
using Microsoft.EntityFrameworkCore;

namespace ApprenticeManager.Services;

public class LearningJournalService : ILearningJournalService
{
    private readonly AppDbContext _db;

    public LearningJournalService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<LearningJournal>> GetByApprenticeAsync(int apprenticeId)
        => await _db.LearningJournals
            .Where(j => j.ApprenticeId == apprenticeId)
            .OrderByDescending(j => j.Jahr)
            .ThenByDescending(j => j.Kalenderwoche)
            .ToListAsync();

    public async Task<LearningJournal?> GetByIdAsync(int id)
        => await _db.LearningJournals.FindAsync(id);

    public async Task<LearningJournal> AddAsync(LearningJournal journal)
    {
        journal.ErstelltAm = DateTime.UtcNow;
        _db.LearningJournals.Add(journal);
        await _db.SaveChangesAsync();
        return journal;
    }

    public async Task<LearningJournal> UpdateAsync(LearningJournal journal)
    {
        journal.GeaendertAm = DateTime.UtcNow;
        _db.LearningJournals.Update(journal);
        await _db.SaveChangesAsync();
        return journal;
    }

    public async Task DeleteAsync(int id)
    {
        var journal = await _db.LearningJournals.FindAsync(id)
            ?? throw new InvalidOperationException($"Journaleintrag mit ID {id} nicht gefunden.");
        _db.LearningJournals.Remove(journal);
        await _db.SaveChangesAsync();
    }
}
