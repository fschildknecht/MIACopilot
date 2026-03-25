using ApprenticeManager.Data;
using ApprenticeManager.Models;
using Microsoft.EntityFrameworkCore;

namespace ApprenticeManager.Services;

/// <summary>EF Core–backed service for vocational trainer CRUD operations.</summary>
public class VocationalTrainerService : IVocationalTrainerService
{
    private readonly AppDbContext _db;

    public VocationalTrainerService(AppDbContext db) => _db = db;

    /// <summary>Returns all vocational trainers ordered alphabetically by last name.</summary>
    public async Task<IReadOnlyList<VocationalTrainer>> GetAllAsync()
        => await _db.VocationalTrainers
            .OrderBy(t => t.LastName)
            .ThenBy(t => t.FirstName)
            .ToListAsync();

    /// <summary>Returns a single trainer by primary key, or null if not found.</summary>
    public async Task<VocationalTrainer?> GetByIdAsync(int id)
        => await _db.VocationalTrainers.FindAsync(id);

    /// <summary>Persists a new vocational trainer and returns the saved entity.</summary>
    public async Task<VocationalTrainer> AddAsync(VocationalTrainer trainer)
    {
        trainer.CreatedAt = DateTime.UtcNow;
        _db.VocationalTrainers.Add(trainer);
        await _db.SaveChangesAsync();
        return trainer;
    }

    /// <summary>Updates an existing vocational trainer record.</summary>
    public async Task<VocationalTrainer> UpdateAsync(VocationalTrainer trainer)
    {
        _db.VocationalTrainers.Update(trainer);
        await _db.SaveChangesAsync();
        return trainer;
    }

    /// <summary>Deletes a vocational trainer by ID; throws if the record does not exist.</summary>
    public async Task DeleteAsync(int id)
    {
        var trainer = await _db.VocationalTrainers.FindAsync(id)
            ?? throw new InvalidOperationException($"Lehrlingsbetreuer mit ID {id} nicht gefunden.");
        _db.VocationalTrainers.Remove(trainer);
        await _db.SaveChangesAsync();
    }
}
