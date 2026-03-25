using ApprenticeManager.Data;
using ApprenticeManager.Models;
using Microsoft.EntityFrameworkCore;

namespace ApprenticeManager.Services;

public class ApprenticeService : IApprenticeService
{
    private readonly AppDbContext _db;

    public ApprenticeService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Apprentice>> GetAllAsync()
    {
        return await _db.Apprentices
            .OrderBy(a => a.LastName)
            .ThenBy(a => a.FirstName)
            .ToListAsync();
    }

    public async Task<Apprentice?> GetByIdAsync(int id)
    {
        return await _db.Apprentices.FindAsync(id);
    }

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

    public async Task<Apprentice> AddAsync(Apprentice apprentice)
    {
        apprentice.CreatedAt = DateTime.UtcNow;
        _db.Apprentices.Add(apprentice);
        await _db.SaveChangesAsync();
        return apprentice;
    }

    public async Task<Apprentice> UpdateAsync(Apprentice apprentice)
    {
        _db.Apprentices.Update(apprentice);
        await _db.SaveChangesAsync();
        return apprentice;
    }

    public async Task DeleteAsync(int id)
    {
        var apprentice = await _db.Apprentices.FindAsync(id)
            ?? throw new InvalidOperationException($"Apprentice with ID {id} not found.");
        _db.Apprentices.Remove(apprentice);
        await _db.SaveChangesAsync();
    }
}
