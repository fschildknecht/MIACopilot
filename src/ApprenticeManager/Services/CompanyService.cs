using ApprenticeManager.Data;
using ApprenticeManager.Models;
using Microsoft.EntityFrameworkCore;

namespace ApprenticeManager.Services;

/// <summary>EF Core–backed service for company CRUD operations.</summary>
public class CompanyService : ICompanyService
{
    private readonly AppDbContext _db;

    public CompanyService(AppDbContext db) => _db = db;

    /// <summary>Returns all companies ordered alphabetically by name.</summary>
    public async Task<IReadOnlyList<Company>> GetAllAsync()
        => await _db.Companies
            .OrderBy(c => c.Name)
            .ToListAsync();

    /// <summary>Returns a single company by primary key, or null if not found.</summary>
    public async Task<Company?> GetByIdAsync(int id)
        => await _db.Companies.FindAsync(id);

    /// <summary>Persists a new company and returns the saved entity.</summary>
    public async Task<Company> AddAsync(Company company)
    {
        company.CreatedAt = DateTime.UtcNow;
        _db.Companies.Add(company);
        await _db.SaveChangesAsync();
        return company;
    }

    /// <summary>Updates an existing company record.</summary>
    public async Task<Company> UpdateAsync(Company company)
    {
        _db.Companies.Update(company);
        await _db.SaveChangesAsync();
        return company;
    }

    /// <summary>Deletes a company by ID; throws if the record does not exist.</summary>
    public async Task DeleteAsync(int id)
    {
        var company = await _db.Companies.FindAsync(id)
            ?? throw new InvalidOperationException($"Betrieb mit ID {id} nicht gefunden.");
        _db.Companies.Remove(company);
        await _db.SaveChangesAsync();
    }
}
