using ApprenticeManager.Models;

namespace ApprenticeManager.Services;

public interface ILearningJournalService
{
    Task<IReadOnlyList<LearningJournal>> GetByApprenticeAsync(int apprenticeId);
    Task<LearningJournal?> GetByIdAsync(int id);
    Task<LearningJournal> AddAsync(LearningJournal journal);
    Task<LearningJournal> UpdateAsync(LearningJournal journal);
    Task DeleteAsync(int id);
}
