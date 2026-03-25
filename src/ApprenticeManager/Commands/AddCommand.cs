using ApprenticeManager.Models;
using ApprenticeManager.Services;
using ApprenticeManager.Validators;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ApprenticeManager.Commands;

public class AddCommand : AsyncCommand
{
    private readonly IApprenticeService _service;
    private readonly ApprenticeValidator _validator;

    public AddCommand(IApprenticeService service, ApprenticeValidator validator)
    {
        _service = service;
        _validator = validator;
    }

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        AnsiConsole.MarkupLine("[blue]Add New Apprentice[/]");
        AnsiConsole.WriteLine();

        var apprentice = PromptForApprentice(new Apprentice());

        var validation = await _validator.ValidateAsync(apprentice);
        if (!validation.IsValid)
        {
            AnsiConsole.MarkupLine("[red]Validation errors:[/]");
            foreach (var error in validation.Errors)
            {
                AnsiConsole.MarkupLine($"  [red]- {Markup.Escape(error.ErrorMessage)}[/]");
            }
            return 1;
        }

        Apprentice? saved = null;
        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .StartAsync("[blue]Saving...[/]", async _ =>
            {
                saved = await _service.AddAsync(apprentice);
            });

        AnsiConsole.MarkupLine($"[green]Apprentice [bold]{Markup.Escape(saved!.FullName)}[/] added with ID {saved.Id}.[/]");
        return 0;
    }

    internal static Apprentice PromptForApprentice(Apprentice existing)
    {
        existing.FirstName = AnsiConsole.Ask<string>(
            $"[yellow]First name[/]{(existing.FirstName.Length > 0 ? $" [[{Markup.Escape(existing.FirstName)}]]" : "")}:");

        existing.LastName = AnsiConsole.Ask<string>(
            $"[yellow]Last name[/]{(existing.LastName.Length > 0 ? $" [[{Markup.Escape(existing.LastName)}]]" : "")}:");

        existing.DateOfBirth = PromptDate("Date of birth (dd.MM.yyyy)", existing.DateOfBirth == default ? null : existing.DateOfBirth);
        existing.StartDate = PromptDate("Start date (dd.MM.yyyy)", existing.StartDate == default ? null : existing.StartDate);
        existing.EndDate = PromptDate("End date (dd.MM.yyyy)", existing.EndDate == default ? null : existing.EndDate);

        existing.Occupation = AnsiConsole.Ask<string>(
            $"[yellow]Occupation[/]{(existing.Occupation.Length > 0 ? $" [[{Markup.Escape(existing.Occupation)}]]" : "")}:");

        existing.Company = AnsiConsole.Ask<string>(
            $"[yellow]Company[/]{(existing.Company.Length > 0 ? $" [[{Markup.Escape(existing.Company)}]]" : "")}:");

        var emailPrompt = new TextPrompt<string>("[yellow]Email[/] (optional):")
            .AllowEmpty();
        if (!string.IsNullOrEmpty(existing.Email))
            emailPrompt.DefaultValue(existing.Email);
        existing.Email = AnsiConsole.Prompt(emailPrompt).NullIfEmpty();

        var phonePrompt = new TextPrompt<string>("[yellow]Phone[/] (optional):")
            .AllowEmpty();
        if (!string.IsNullOrEmpty(existing.Phone))
            phonePrompt.DefaultValue(existing.Phone);
        existing.Phone = AnsiConsole.Prompt(phonePrompt).NullIfEmpty();

        var notesPrompt = new TextPrompt<string>("[yellow]Notes[/] (optional):")
            .AllowEmpty();
        if (!string.IsNullOrEmpty(existing.Notes))
            notesPrompt.DefaultValue(existing.Notes);
        existing.Notes = AnsiConsole.Prompt(notesPrompt).NullIfEmpty();

        return existing;
    }

    private static DateOnly PromptDate(string label, DateOnly? defaultValue)
    {
        while (true)
        {
            var hint = defaultValue.HasValue ? $" [[{defaultValue.Value:dd.MM.yyyy}]]" : "";
            var input = AnsiConsole.Ask<string>($"[yellow]{label}[/]{hint}:");

            if (string.IsNullOrWhiteSpace(input) && defaultValue.HasValue)
                return defaultValue.Value;

            if (DateOnly.TryParseExact(input, "dd.MM.yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var date))
            {
                return date;
            }

            AnsiConsole.MarkupLine("[red]Invalid date format. Please use dd.MM.yyyy.[/]");
        }
    }
}

internal static class StringExtensions
{
    public static string? NullIfEmpty(this string value)
        => string.IsNullOrWhiteSpace(value) ? null : value;
}
