using ApprenticeManager.Data;
using ApprenticeManager.Models;
using ApprenticeManager.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ApprenticeManager.Tests;

public class ApprenticeServiceTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly ApprenticeService _service;

    public ApprenticeServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);
        _service = new ApprenticeService(_db);
    }

    public void Dispose() => _db.Dispose();

    private static Apprentice CreateSampleApprentice(
        string firstName = "Max",
        string lastName = "Muster",
        string company = "ACME AG") => new()
    {
        FirstName = firstName,
        LastName = lastName,
        DateOfBirth = new DateOnly(2000, 1, 1),
        StartDate = new DateOnly(2023, 8, 1),
        EndDate = new DateOnly(2026, 7, 31),
        Occupation = "Informatiker EFZ",
        Company = company,
    };

    [Fact]
    public async Task AddAsync_ShouldPersistApprentice()
    {
        var apprentice = CreateSampleApprentice();

        var saved = await _service.AddAsync(apprentice);

        saved.Id.Should().BeGreaterThan(0);
        saved.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        var fromDb = await _db.Apprentices.FindAsync(saved.Id);
        fromDb.Should().NotBeNull();
        fromDb!.FirstName.Should().Be("Max");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllApprentices()
    {
        await _service.AddAsync(CreateSampleApprentice("Alice", "Smith"));
        await _service.AddAsync(CreateSampleApprentice("Bob", "Brown"));

        var all = await _service.GetAllAsync();

        all.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOrderedByLastNameThenFirstName()
    {
        await _service.AddAsync(CreateSampleApprentice("Zoe", "Adams"));
        await _service.AddAsync(CreateSampleApprentice("Alice", "Adams"));
        await _service.AddAsync(CreateSampleApprentice("Bob", "Baker"));

        var all = await _service.GetAllAsync();

        all[0].FirstName.Should().Be("Alice");
        all[1].FirstName.Should().Be("Zoe");
        all[2].FirstName.Should().Be("Bob");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectApprentice()
    {
        var saved = await _service.AddAsync(CreateSampleApprentice());

        var found = await _service.GetByIdAsync(saved.Id);

        found.Should().NotBeNull();
        found!.FirstName.Should().Be("Max");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNullForMissingId()
    {
        var found = await _service.GetByIdAsync(9999);

        found.Should().BeNull();
    }

    [Fact]
    public async Task SearchAsync_ShouldFindByFirstName()
    {
        await _service.AddAsync(CreateSampleApprentice("Alice", "Smith"));
        await _service.AddAsync(CreateSampleApprentice("Bob", "Jones"));

        var results = await _service.SearchAsync("alice");

        results.Should().HaveCount(1);
        results[0].FirstName.Should().Be("Alice");
    }

    [Fact]
    public async Task SearchAsync_ShouldFindByCompany()
    {
        await _service.AddAsync(CreateSampleApprentice(company: "TechCorp AG"));
        await _service.AddAsync(CreateSampleApprentice(company: "OtherCorp"));

        var results = await _service.SearchAsync("techcorp");

        results.Should().HaveCount(1);
        results[0].Company.Should().Be("TechCorp AG");
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnEmptyForNoMatch()
    {
        await _service.AddAsync(CreateSampleApprentice());

        var results = await _service.SearchAsync("xyz_no_match_123");

        results.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyExistingApprentice()
    {
        var saved = await _service.AddAsync(CreateSampleApprentice());
        saved.FirstName = "Updated";

        await _service.UpdateAsync(saved);

        var updated = await _service.GetByIdAsync(saved.Id);
        updated!.FirstName.Should().Be("Updated");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveApprentice()
    {
        var saved = await _service.AddAsync(CreateSampleApprentice());

        await _service.DeleteAsync(saved.Id);

        var found = await _service.GetByIdAsync(saved.Id);
        found.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowForNonExistentId()
    {
        var act = async () => await _service.DeleteAsync(9999);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*9999*");
    }
}
