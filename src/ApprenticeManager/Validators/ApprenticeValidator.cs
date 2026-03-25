using ApprenticeManager.Models;
using FluentValidation;

namespace ApprenticeManager.Validators;

public class ApprenticeValidator : AbstractValidator<Apprentice>
{
    public ApprenticeValidator()
    {
        RuleFor(a => a.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters.");

        RuleFor(a => a.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters.");

        RuleFor(a => a.DateOfBirth)
            .Must(dob => dob < DateOnly.FromDateTime(DateTime.Today))
                .WithMessage("Date of birth must be in the past.")
            .Must(dob => dob <= DateOnly.FromDateTime(DateTime.Today).AddYears(-15))
                .WithMessage("Apprentice must be at least 15 years old.");

        RuleFor(a => a.StartDate)
            .NotEmpty().WithMessage("Start date is required.");

        RuleFor(a => a.EndDate)
            .NotEmpty().WithMessage("End date is required.")
            .GreaterThan(a => a.StartDate)
                .WithMessage("End date must be after start date.");

        RuleFor(a => a.Occupation)
            .NotEmpty().WithMessage("Occupation is required.");

        RuleFor(a => a.Company)
            .NotEmpty().WithMessage("Company is required.");

        RuleFor(a => a.Email)
            .EmailAddress().WithMessage("Email must be a valid email address.")
            .When(a => !string.IsNullOrWhiteSpace(a.Email));
    }
}
