using ApprenticeManager.Models;

namespace ApprenticeManager.Services;

/// <summary>Contract for managing vocational trainer data.</summary>
public interface IVocationalTrainerService
{
    Task<IReadOnlyList<VocationalTrainer>> GetAllAsync();
    Task<VocationalTrainer?> GetByIdAsync(int id);
    Task<VocationalTrainer> AddAsync(VocationalTrainer trainer);
    Task<VocationalTrainer> UpdateAsync(VocationalTrainer trainer);
    Task DeleteAsync(int id);
}
