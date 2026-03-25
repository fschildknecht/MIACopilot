using ApprenticeManager.Data;
using ApprenticeManager.Models;
using Microsoft.EntityFrameworkCore;

namespace ApprenticeManager.Services;

public class GradeService : IGradeService
{
    private readonly AppDbContext _db;

    public GradeService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<Subject>> GetSubjectsByApprenticeAsync(int apprenticeId)
        => await _db.Subjects
            .Include(s => s.Grades)
            .Where(s => s.ApprenticeId == apprenticeId)
            .OrderBy(s => s.Name)
            .ToListAsync();

    public async Task<Subject?> GetSubjectByIdAsync(int subjectId)
        => await _db.Subjects.Include(s => s.Grades).FirstOrDefaultAsync(s => s.Id == subjectId);

    public async Task<Subject> AddSubjectAsync(Subject subject)
    {
        _db.Subjects.Add(subject);
        await _db.SaveChangesAsync();
        return subject;
    }

    public async Task DeleteSubjectAsync(int id)
    {
        var subject = await _db.Subjects.FindAsync(id)
            ?? throw new InvalidOperationException($"Fach mit ID {id} nicht gefunden.");
        _db.Subjects.Remove(subject);
        await _db.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Grade>> GetGradesBySubjectAsync(int subjectId)
        => await _db.Grades
            .Where(g => g.SubjectId == subjectId)
            .OrderByDescending(g => g.Date)
            .ToListAsync();

    public async Task<Grade> AddGradeAsync(Grade grade)
    {
        grade.CreatedAt = DateTime.UtcNow;
        _db.Grades.Add(grade);
        await _db.SaveChangesAsync();
        return grade;
    }

    public async Task<Grade> UpdateGradeAsync(Grade grade)
    {
        _db.Grades.Update(grade);
        await _db.SaveChangesAsync();
        return grade;
    }

    public async Task DeleteGradeAsync(int id)
    {
        var grade = await _db.Grades.FindAsync(id)
            ?? throw new InvalidOperationException($"Note mit ID {id} nicht gefunden.");
        _db.Grades.Remove(grade);
        await _db.SaveChangesAsync();
    }
}
