using ApprenticeManager.Data;
using ApprenticeManager.Models;
using Microsoft.EntityFrameworkCore;

namespace ApprenticeManager.Services;

/// <summary>EF Core–backed service for learning journal CRUD operations.</summary>
public class LearningJournalService : ILearningJournalService
{
    private readonly AppDbContext _db;

    public LearningJournalService(AppDbContext db) => _db = db;

    /// <summary>Returns all journal entries for the given apprentice, newest first.</summary>
    public async Task<IReadOnlyList<LearningJournal>> GetByApprenticeAsync(int apprenticeId)
        => await _db.LearningJournals
            .Where(j => j.ApprenticeId == apprenticeId)
            .OrderByDescending(j => j.Jahr)
            .ThenByDescending(j => j.Kalenderwoche)
            .ToListAsync();

    /// <summary>Returns a single journal entry by primary key, or null if not found.</summary>
    public async Task<LearningJournal?> GetByIdAsync(int id)
        => await _db.LearningJournals.FindAsync(id);

    /// <summary>Persists a new journal entry and returns the saved entity.</summary>
    public async Task<LearningJournal> AddAsync(LearningJournal journal)
    {
        journal.ErstelltAm = DateTime.UtcNow;
        _db.LearningJournals.Add(journal);
        await _db.SaveChangesAsync();
        return journal;
    }

    /// <summary>Updates an existing journal entry and sets the modification timestamp.</summary>
    public async Task<LearningJournal> UpdateAsync(LearningJournal journal)
    {
        journal.GeaendertAm = DateTime.UtcNow;
        _db.LearningJournals.Update(journal);
        await _db.SaveChangesAsync();
        return journal;
    }

    /// <summary>Deletes a journal entry by ID; throws if the record does not exist.</summary>
    public async Task DeleteAsync(int id)
    {
        var journal = await _db.LearningJournals.FindAsync(id)
            ?? throw new InvalidOperationException($"Journaleintrag mit ID {id} nicht gefunden.");
        _db.LearningJournals.Remove(journal);
        await _db.SaveChangesAsync();
    }
}
