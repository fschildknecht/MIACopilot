using ApprenticeManager.Models;

namespace ApprenticeManager.Services;

public interface IApprenticeService
{
    Task<IReadOnlyList<Apprentice>> GetAllAsync();
    Task<Apprentice?> GetByIdAsync(int id);
    Task<IReadOnlyList<Apprentice>> SearchAsync(string term);
    Task<Apprentice> AddAsync(Apprentice apprentice);
    Task<Apprentice> UpdateAsync(Apprentice apprentice);
    Task DeleteAsync(int id);
}
