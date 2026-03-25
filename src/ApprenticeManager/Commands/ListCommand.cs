using ApprenticeManager.Services;
using ApprenticeManager.UI;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ApprenticeManager.Commands;

public class ListCommand : AsyncCommand
{
    private readonly IApprenticeService _service;

    public ListCommand(IApprenticeService service)
    {
        _service = service;
    }

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        IReadOnlyList<Models.Apprentice> apprentices = [];

        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .StartAsync("[blue]Loading apprentices...[/]", async _ =>
            {
                apprentices = await _service.GetAllAsync();
            });

        if (apprentices.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No apprentices found.[/]");
            return 0;
        }

        AnsiConsole.MarkupLine($"[blue]Found [bold]{apprentices.Count}[/] apprentice(s).[/]");
        TableRenderer.RenderApprentices(apprentices);
        return 0;
    }
}
