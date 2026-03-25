using ApprenticeManager.Models;
using Microsoft.EntityFrameworkCore;

namespace ApprenticeManager.Data;

/// <summary>EF Core database context for the Apprentice Manager application.</summary>
public class AppDbContext : DbContext
{
    public const string ConnectionString = "Data Source=apprentices.db";

    public DbSet<Apprentice> Apprentices => Set<Apprentice>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Grade> Grades => Set<Grade>();
    public DbSet<LearningJournal> LearningJournals => Set<LearningJournal>();
    public DbSet<VocationalTrainer> VocationalTrainers => Set<VocationalTrainer>();
    public DbSet<Company> Companies => Set<Company>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Apprentice>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.DateOfBirth)
                .IsRequired();

            entity.Property(e => e.StartDate)
                .IsRequired();

            entity.Property(e => e.EndDate)
                .IsRequired();

            entity.Property(e => e.Occupation)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Company)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Email)
                .HasMaxLength(200);

            entity.Property(e => e.Phone)
                .HasMaxLength(50);

            entity.Property(e => e.Notes)
                .HasMaxLength(2000);

            entity.Property(e => e.CreatedAt)
                .IsRequired();
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.HasOne(e => e.Apprentice)
                .WithMany(a => a.Subjects)
                .HasForeignKey(e => e.ApprenticeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Value).IsRequired().HasPrecision(3, 1);
            entity.HasOne(e => e.Subject)
                .WithMany(s => s.Grades)
                .HasForeignKey(e => e.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<LearningJournal>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.BetrieblicheTaetigkeiten).IsRequired().HasMaxLength(5000);
            entity.Property(e => e.SchulischeTaetigkeiten).IsRequired().HasMaxLength(5000);
            entity.Property(e => e.Reflexion).IsRequired().HasMaxLength(3000);
            entity.HasOne(e => e.Apprentice)
                .WithMany(a => a.LearningJournals)
                .HasForeignKey(e => e.ApprenticeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<VocationalTrainer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Company).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Address).HasMaxLength(400);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.Property(e => e.CreatedAt).IsRequired();
        });
    }
}
