using ApprenticeManager.Models;
using ApprenticeManager.Validators;
using FluentAssertions;
using Xunit;

namespace ApprenticeManager.Tests;

public class ApprenticeValidatorTests
{
    private readonly ApprenticeValidator _validator = new();

    private static Apprentice ValidApprentice() => new()
    {
        FirstName = "Max",
        LastName = "Muster",
        DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-20)),
        StartDate = new DateOnly(2023, 8, 1),
        EndDate = new DateOnly(2026, 7, 31),
        Occupation = "Informatiker EFZ",
        Company = "ACME AG",
        Email = null,
        Phone = null,
        Notes = null,
    };

    [Fact]
    public async Task Validate_ValidApprentice_ShouldPass()
    {
        var result = await _validator.ValidateAsync(ValidApprentice());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyFirstName_ShouldFail()
    {
        var apprentice = ValidApprentice();
        apprentice.FirstName = "";

        var result = await _validator.ValidateAsync(apprentice);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FirstName");
    }

    [Fact]
    public async Task Validate_FirstNameTooLong_ShouldFail()
    {
        var apprentice = ValidApprentice();
        apprentice.FirstName = new string('A', 101);

        var result = await _validator.ValidateAsync(apprentice);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FirstName");
    }

    [Fact]
    public async Task Validate_EmptyLastName_ShouldFail()
    {
        var apprentice = ValidApprentice();
        apprentice.LastName = "";

        var result = await _validator.ValidateAsync(apprentice);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "LastName");
    }

    [Fact]
    public async Task Validate_LastNameTooLong_ShouldFail()
    {
        var apprentice = ValidApprentice();
        apprentice.LastName = new string('B', 101);

        var result = await _validator.ValidateAsync(apprentice);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "LastName");
    }

    [Fact]
    public async Task Validate_DateOfBirthInFuture_ShouldFail()
    {
        var apprentice = ValidApprentice();
        apprentice.DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

        var result = await _validator.ValidateAsync(apprentice);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DateOfBirth");
    }

    [Fact]
    public async Task Validate_DateOfBirthTooYoung_ShouldFail()
    {
        var apprentice = ValidApprentice();
        apprentice.DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-14));

        var result = await _validator.ValidateAsync(apprentice);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DateOfBirth");
    }

    [Fact]
    public async Task Validate_DateOfBirthExactly15_ShouldPass()
    {
        var apprentice = ValidApprentice();
        // Exactly 15 years ago today - valid
        apprentice.DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-15));

        var result = await _validator.ValidateAsync(apprentice);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EndDateBeforeStartDate_ShouldFail()
    {
        var apprentice = ValidApprentice();
        apprentice.StartDate = new DateOnly(2024, 8, 1);
        apprentice.EndDate = new DateOnly(2023, 7, 31);

        var result = await _validator.ValidateAsync(apprentice);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "EndDate");
    }

    [Fact]
    public async Task Validate_StartDateEqualsEndDate_ShouldFail()
    {
        var apprentice = ValidApprentice();
        apprentice.StartDate = new DateOnly(2024, 8, 1);
        apprentice.EndDate = new DateOnly(2024, 8, 1);

        var result = await _validator.ValidateAsync(apprentice);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "EndDate");
    }

    [Fact]
    public async Task Validate_ValidEmail_ShouldPass()
    {
        var apprentice = ValidApprentice();
        apprentice.Email = "test@example.com";

        var result = await _validator.ValidateAsync(apprentice);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_InvalidEmail_ShouldFail()
    {
        var apprentice = ValidApprentice();
        apprentice.Email = "not-a-valid-email";

        var result = await _validator.ValidateAsync(apprentice);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public async Task Validate_NullEmail_ShouldPass()
    {
        var apprentice = ValidApprentice();
        apprentice.Email = null;

        var result = await _validator.ValidateAsync(apprentice);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyOccupation_ShouldFail()
    {
        var apprentice = ValidApprentice();
        apprentice.Occupation = "";

        var result = await _validator.ValidateAsync(apprentice);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Occupation");
    }

    [Fact]
    public async Task Validate_EmptyCompany_ShouldFail()
    {
        var apprentice = ValidApprentice();
        apprentice.Company = "";

        var result = await _validator.ValidateAsync(apprentice);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Company");
    }
}
