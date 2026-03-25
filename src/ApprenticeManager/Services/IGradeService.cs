using ApprenticeManager.Models;

namespace ApprenticeManager.Services;

public interface IGradeService
{
    Task<IReadOnlyList<Subject>> GetSubjectsByApprenticeAsync(int apprenticeId);
    Task<Subject?> GetSubjectByIdAsync(int subjectId);
    Task<Subject> AddSubjectAsync(Subject subject);
    Task DeleteSubjectAsync(int id);

    Task<IReadOnlyList<Grade>> GetGradesBySubjectAsync(int subjectId);
    Task<Grade> AddGradeAsync(Grade grade);
    Task<Grade> UpdateGradeAsync(Grade grade);
    Task DeleteGradeAsync(int id);
}
