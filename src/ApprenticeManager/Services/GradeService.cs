using ApprenticeManager.Data;
using ApprenticeManager.Models;
using Microsoft.EntityFrameworkCore;

namespace ApprenticeManager.Services;

/// <summary>EF Core–backed service for subject and grade CRUD operations.</summary>
public class GradeService : IGradeService
{
    private readonly AppDbContext _db;

    public GradeService(AppDbContext db) => _db = db;

    /// <summary>Returns all subjects with their grades for the given apprentice.</summary>
    public async Task<IReadOnlyList<Subject>> GetSubjectsByApprenticeAsync(int apprenticeId)
        => await _db.Subjects
            .Include(s => s.Grades)
            .Where(s => s.ApprenticeId == apprenticeId)
            .OrderBy(s => s.Name)
            .ToListAsync();

    /// <summary>Returns a subject with its grades by ID, or null if not found.</summary>
    public async Task<Subject?> GetSubjectByIdAsync(int subjectId)
        => await _db.Subjects.Include(s => s.Grades).FirstOrDefaultAsync(s => s.Id == subjectId);

    /// <summary>Persists a new subject and returns the saved entity.</summary>
    public async Task<Subject> AddSubjectAsync(Subject subject)
    {
        _db.Subjects.Add(subject);
        await _db.SaveChangesAsync();
        return subject;
    }

    /// <summary>Deletes a subject and all its grades by ID; throws if not found.</summary>
    public async Task DeleteSubjectAsync(int id)
    {
        var subject = await _db.Subjects.FindAsync(id)
            ?? throw new InvalidOperationException($"Fach mit ID {id} nicht gefunden.");
        _db.Subjects.Remove(subject);
        await _db.SaveChangesAsync();
    }

    /// <summary>Returns all grades for the given subject, newest first.</summary>
    public async Task<IReadOnlyList<Grade>> GetGradesBySubjectAsync(int subjectId)
        => await _db.Grades
            .Where(g => g.SubjectId == subjectId)
            .OrderByDescending(g => g.Date)
            .ToListAsync();

    /// <summary>Persists a new grade and returns the saved entity.</summary>
    public async Task<Grade> AddGradeAsync(Grade grade)
    {
        grade.CreatedAt = DateTime.UtcNow;
        _db.Grades.Add(grade);
        await _db.SaveChangesAsync();
        return grade;
    }

    /// <summary>Updates an existing grade record.</summary>
    public async Task<Grade> UpdateGradeAsync(Grade grade)
    {
        _db.Grades.Update(grade);
        await _db.SaveChangesAsync();
        return grade;
    }

    /// <summary>Deletes a grade by ID; throws if the record does not exist.</summary>
    public async Task DeleteGradeAsync(int id)
    {
        var grade = await _db.Grades.FindAsync(id)
            ?? throw new InvalidOperationException($"Note mit ID {id} nicht gefunden.");
        _db.Grades.Remove(grade);
        await _db.SaveChangesAsync();
    }
}
