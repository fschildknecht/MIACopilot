using ApprenticeManager.Models;
using Spectre.Console;

namespace ApprenticeManager.UI;

public static class TableRenderer
{
    public static void RenderApprentices(IReadOnlyList<Apprentice> apprentices)
    {
        var table = new Table();
        table.Border = TableBorder.Rounded;
        table.AddColumn(new TableColumn("[bold blue]ID[/]").Centered());
        table.AddColumn(new TableColumn("[bold blue]Name[/]"));
        table.AddColumn(new TableColumn("[bold blue]Date of Birth[/]").Centered());
        table.AddColumn(new TableColumn("[bold blue]Company[/]"));
        table.AddColumn(new TableColumn("[bold blue]Occupation[/]"));
        table.AddColumn(new TableColumn("[bold blue]Start[/]").Centered());
        table.AddColumn(new TableColumn("[bold blue]End[/]").Centered());
        table.AddColumn(new TableColumn("[bold blue]Email[/]"));

        foreach (var a in apprentices)
        {
            table.AddRow(
                a.Id.ToString(),
                Markup.Escape(a.FullName),
                a.DateOfBirth.ToString("dd.MM.yyyy"),
                Markup.Escape(a.Company),
                Markup.Escape(a.Occupation),
                a.StartDate.ToString("dd.MM.yyyy"),
                a.EndDate.ToString("dd.MM.yyyy"),
                Markup.Escape(a.Email ?? string.Empty)
            );
        }

        AnsiConsole.Write(table);
    }

    public static void RenderApprenticeDetail(Apprentice a)
    {
        var table = new Table();
        table.Border = TableBorder.Rounded;
        table.AddColumn(new TableColumn("[bold blue]Field[/]"));
        table.AddColumn(new TableColumn("[bold blue]Value[/]"));

        table.AddRow("ID", a.Id.ToString());
        table.AddRow("First Name", Markup.Escape(a.FirstName));
        table.AddRow("Last Name", Markup.Escape(a.LastName));
        table.AddRow("Date of Birth", a.DateOfBirth.ToString("dd.MM.yyyy"));
        table.AddRow("Company", Markup.Escape(a.Company));
        table.AddRow("Occupation", Markup.Escape(a.Occupation));
        table.AddRow("Start Date", a.StartDate.ToString("dd.MM.yyyy"));
        table.AddRow("End Date", a.EndDate.ToString("dd.MM.yyyy"));
        table.AddRow("Email", Markup.Escape(a.Email ?? "-"));
        table.AddRow("Phone", Markup.Escape(a.Phone ?? "-"));
        table.AddRow("Notes", Markup.Escape(a.Notes ?? "-"));
        table.AddRow("Created At", a.CreatedAt.ToString("dd.MM.yyyy HH:mm"));

        AnsiConsole.Write(table);
    }

    public static void PrintBanner()
    {
        AnsiConsole.Write(
            new FigletText("Apprentice Manager")
                .LeftJustified()
                .Color(Color.Blue));
    }
}
