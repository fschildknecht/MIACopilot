using ApprenticeManager.Services;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace ApprenticeManager.Commands;

public class DeleteSettings : CommandSettings
{
    [CommandArgument(0, "<id>")]
    [Description("The ID of the apprentice to delete.")]
    public int Id { get; set; }
}

public class DeleteCommand : AsyncCommand<DeleteSettings>
{
    private readonly IApprenticeService _service;

    public DeleteCommand(IApprenticeService service)
    {
        _service = service;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, DeleteSettings settings)
    {
        Models.Apprentice? apprentice = null;

        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .StartAsync("[blue]Loading apprentice...[/]", async _ =>
            {
                apprentice = await _service.GetByIdAsync(settings.Id);
            });

        if (apprentice is null)
        {
            AnsiConsole.MarkupLine($"[red]Apprentice with ID {settings.Id} not found.[/]");
            return 1;
        }

        AnsiConsole.MarkupLine($"[yellow]You are about to delete: [bold]{Markup.Escape(apprentice.FullName)}[/] (ID: {apprentice.Id})[/]");

        if (!AnsiConsole.Confirm("[red]Are you sure you want to delete this apprentice?[/]"))
        {
            AnsiConsole.MarkupLine("[yellow]Delete cancelled.[/]");
            return 0;
        }

        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .StartAsync("[blue]Deleting...[/]", async _ =>
            {
                await _service.DeleteAsync(settings.Id);
            });

        AnsiConsole.MarkupLine($"[green]Apprentice [bold]{Markup.Escape(apprentice.FullName)}[/] deleted successfully.[/]");
        return 0;
    }
}
