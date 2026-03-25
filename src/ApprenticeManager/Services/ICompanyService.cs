using ApprenticeManager.Models;

namespace ApprenticeManager.Services;

/// <summary>Contract for managing company data.</summary>
public interface ICompanyService
{
    Task<IReadOnlyList<Company>> GetAllAsync();
    Task<Company?> GetByIdAsync(int id);
    Task<Company> AddAsync(Company company);
    Task<Company> UpdateAsync(Company company);
    Task DeleteAsync(int id);
}
