using ApprenticeManager.Models;
using ApprenticeManager.Services;
using ApprenticeManager.UI;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ApprenticeManager.Commands;

public class DefaultCommand : AsyncCommand
{
    private readonly IApprenticeService _service;
    private readonly ApprenticeManager.Validators.ApprenticeValidator _validator;
    private readonly IGradeService _gradeService;

    public DefaultCommand(IApprenticeService service,
        ApprenticeManager.Validators.ApprenticeValidator validator,
        IGradeService gradeService)
    {
        _service = service;
        _validator = validator;
        _gradeService = gradeService;
    }

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        TableRenderer.PrintBanner();

        while (true)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[blue]Was möchten Sie tun?[/]")
                    .AddChoices(
                        "Lernende anzeigen",
                        "Lernenden hinzufügen",
                        "Lernenden bearbeiten",
                        "Lernenden löschen",
                        "Lernende suchen",
                        "Noten verwalten",
                        "Lernjournal (nur via App)",
                        "Beenden"));

            switch (choice)
            {
                case "Lernende anzeigen":
                    await new ListCommand(_service).ExecuteAsync(context);
                    break;

                case "Lernenden hinzufügen":
                    await new AddCommand(_service, _validator).ExecuteAsync(context);
                    break;

                case "Lernenden bearbeiten":
                    var editId = AnsiConsole.Ask<int>("[yellow]Lernenden-ID zum Bearbeiten:[/]");
                    var editSettings = new EditSettings { Id = editId };
                    await new EditCommand(_service, _validator).ExecuteAsync(context, editSettings);
                    break;

                case "Lernenden löschen":
                    var deleteId = AnsiConsole.Ask<int>("[yellow]Lernenden-ID zum Löschen:[/]");
                    var deleteSettings = new DeleteSettings { Id = deleteId };
                    await new DeleteCommand(_service).ExecuteAsync(context, deleteSettings);
                    break;

                case "Lernende suchen":
                    var term = AnsiConsole.Ask<string>("[yellow]Suchbegriff:[/]");
                    var searchSettings = new SearchSettings { Term = term };
                    await new SearchCommand(_service).ExecuteAsync(context, searchSettings);
                    break;

                case "Noten verwalten":
                    await RunGradeMenuAsync(context);
                    break;

                case "Lernjournal (nur via App)":
                    AnsiConsole.MarkupLine("[yellow]Das Lernjournal ist nur über die grafische App verfügbar.[/]");
                    AnsiConsole.MarkupLine("[grey]Starten Sie das Programm neu und wählen Sie 'App öffnen'.[/]");
                    break;

                case "Beenden":
                    AnsiConsole.MarkupLine("[blue]Auf Wiedersehen![/]");
                    return 0;
            }

            AnsiConsole.WriteLine();
        }
    }

    private async Task RunGradeMenuAsync(CommandContext context)
    {
        var apprentices = await _service.GetAllAsync();
        if (!apprentices.Any())
        {
            AnsiConsole.MarkupLine("[yellow]Keine Lernenden gefunden. Bitte zuerst Lernende erfassen.[/]");
            return;
        }

        var apprentice = AnsiConsole.Prompt(
            new SelectionPrompt<Apprentice>()
                .Title("[blue]Lernenden auswählen:[/]")
                .UseConverter(a => a.FullName)
                .AddChoices(apprentices));

        while (true)
        {
            var subjects = await _gradeService.GetSubjectsByApprenticeAsync(apprentice.Id);

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[bold blue]Noten für: {Markup.Escape(apprentice.FullName)}[/]");

            if (subjects.Any())
            {
                var table = new Table().Border(TableBorder.Rounded);
                table.AddColumn(new TableColumn("[bold]Fach[/]"));
                table.AddColumn(new TableColumn("[bold]Ø Note[/]").Centered());
                table.AddColumn(new TableColumn("[bold]Anzahl[/]").Centered());
                foreach (var s in subjects)
                    table.AddRow(
                        Markup.Escape(s.Name),
                        s.AverageGrade.HasValue ? s.AverageGrade.Value.ToString("F2") : "-",
                        s.Grades.Count.ToString());
                AnsiConsole.Write(table);
            }
            else
            {
                AnsiConsole.MarkupLine("[grey]Noch keine Fächer erfasst.[/]");
            }

            var action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[blue]Aktion:[/]")
                    .AddChoices("Fach hinzufügen", "Note hinzufügen", "Alle Noten anzeigen",
                                "Fach löschen", "Zurück"));

            switch (action)
            {
                case "Fach hinzufügen":
                    var subjectName = AnsiConsole.Ask<string>("[yellow]Fachname:[/]");
                    await _gradeService.AddSubjectAsync(new Subject { Name = subjectName, ApprenticeId = apprentice.Id });
                    AnsiConsole.MarkupLine($"[green]Fach '{Markup.Escape(subjectName)}' hinzugefügt.[/]");
                    break;

                case "Note hinzufügen":
                    if (!subjects.Any()) { AnsiConsole.MarkupLine("[yellow]Bitte zuerst ein Fach erstellen.[/]"); break; }
                    var subject = AnsiConsole.Prompt(
                        new SelectionPrompt<Subject>()
                            .Title("[yellow]Fach auswählen:[/]")
                            .UseConverter(s => s.Name)
                            .AddChoices(subjects));
                    var gradeValue = AnsiConsole.Ask<decimal>("[yellow]Note (1.0–6.0):[/]");
                    var gradeDate = AnsiConsole.Ask<string>("[yellow]Datum (dd.MM.yyyy, leer = heute):[/]", "");
                    var date = string.IsNullOrWhiteSpace(gradeDate)
                        ? DateOnly.FromDateTime(DateTime.Today)
                        : DateOnly.ParseExact(gradeDate, "dd.MM.yyyy");
                    var gradeNotes = AnsiConsole.Ask<string>("[yellow]Bemerkungen (optional, leer lassen):[/]", "");
                    await _gradeService.AddGradeAsync(new Grade
                    {
                        SubjectId = subject.Id,
                        Value = gradeValue,
                        Date = date,
                        Notes = string.IsNullOrWhiteSpace(gradeNotes) ? null : gradeNotes
                    });
                    AnsiConsole.MarkupLine($"[green]Note {gradeValue:F1} für '{Markup.Escape(subject.Name)}' gespeichert.[/]");
                    break;

                case "Alle Noten anzeigen":
                    if (!subjects.Any()) { AnsiConsole.MarkupLine("[yellow]Keine Fächer vorhanden.[/]"); break; }
                    foreach (var s in subjects)
                    {
                        var grades = await _gradeService.GetGradesBySubjectAsync(s.Id);
                        AnsiConsole.MarkupLine($"\n[bold]{Markup.Escape(s.Name)}[/] (Ø {(s.AverageGrade.HasValue ? s.AverageGrade.Value.ToString("F2") : "-")})");
                        if (!grades.Any()) { AnsiConsole.MarkupLine("[grey]  Keine Noten.[/]"); continue; }
                        var gt = new Table().Border(TableBorder.Simple);
                        gt.AddColumn("Note"); gt.AddColumn("Datum"); gt.AddColumn("Bemerkungen");
                        foreach (var g in grades)
                            gt.AddRow(g.Value.ToString("F1"), g.Date.ToString("dd.MM.yyyy"), Markup.Escape(g.Notes ?? ""));
                        AnsiConsole.Write(gt);
                    }
                    break;

                case "Fach löschen":
                    if (!subjects.Any()) { AnsiConsole.MarkupLine("[yellow]Keine Fächer vorhanden.[/]"); break; }
                    var delSubject = AnsiConsole.Prompt(
                        new SelectionPrompt<Subject>()
                            .Title("[yellow]Fach zum Löschen:[/]")
                            .UseConverter(s => s.Name)
                            .AddChoices(subjects));
                    if (AnsiConsole.Confirm($"[red]Fach '{Markup.Escape(delSubject.Name)}' und alle Noten löschen?[/]"))
                    {
                        await _gradeService.DeleteSubjectAsync(delSubject.Id);
                        AnsiConsole.MarkupLine("[green]Fach gelöscht.[/]");
                    }
                    break;

                case "Zurück":
                    return;
            }

            AnsiConsole.WriteLine();
        }
    }
}
