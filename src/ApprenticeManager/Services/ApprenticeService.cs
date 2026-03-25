using ApprenticeManager.Data;
using ApprenticeManager.Models;
using Microsoft.EntityFrameworkCore;

namespace ApprenticeManager.Services;

/// <summary>EF Core–backed service for apprentice CRUD and search operations.</summary>
public class ApprenticeService : IApprenticeService
{
    private readonly AppDbContext _db;

    public ApprenticeService(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>Returns all apprentices ordered alphabetically by last name.</summary>
    public async Task<IReadOnlyList<Apprentice>> GetAllAsync()
    {
        return await _db.Apprentices
            .OrderBy(a => a.LastName)
            .ThenBy(a => a.FirstName)
            .ToListAsync();
    }

    /// <summary>Returns a single apprentice by primary key, or null if not found.</summary>
    public async Task<Apprentice?> GetByIdAsync(int id)
    {
        return await _db.Apprentices.FindAsync(id);
    }

    /// <summary>Searches apprentices by name, company, or occupation (case-insensitive).</summary>
    public async Task<IReadOnlyList<Apprentice>> SearchAsync(string term)
    {
        var lower = term.ToLowerInvariant();
        return await _db.Apprentices
            .Where(a =>
                a.FirstName.ToLower().Contains(lower) ||
                a.LastName.ToLower().Contains(lower) ||
                a.Company.ToLower().Contains(lower) ||
                a.Occupation.ToLower().Contains(lower))
            .OrderBy(a => a.LastName)
            .ThenBy(a => a.FirstName)
            .ToListAsync();
    }

    /// <summary>Persists a new apprentice and returns the saved entity.</summary>
    public async Task<Apprentice> AddAsync(Apprentice apprentice)
    {
        apprentice.CreatedAt = DateTime.UtcNow;
        _db.Apprentices.Add(apprentice);
        await _db.SaveChangesAsync();
        return apprentice;
    }

    /// <summary>Updates an existing apprentice record.</summary>
    public async Task<Apprentice> UpdateAsync(Apprentice apprentice)
    {
        _db.Apprentices.Update(apprentice);
        await _db.SaveChangesAsync();
        return apprentice;
    }

    /// <summary>Deletes an apprentice and all related data by ID; throws if not found.</summary>
    public async Task DeleteAsync(int id)
    {
        var apprentice = await _db.Apprentices.FindAsync(id)
            ?? throw new InvalidOperationException($"Apprentice with ID {id} not found.");
        _db.Apprentices.Remove(apprentice);
        await _db.SaveChangesAsync();
    }
}
