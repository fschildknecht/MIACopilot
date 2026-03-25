using ApprenticeManager.Services;
using ApprenticeManager.UI;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace ApprenticeManager.Commands;

public class SearchSettings : CommandSettings
{
    [CommandArgument(0, "<term>")]
    [Description("The search term to look for in name or company.")]
    public string Term { get; set; } = string.Empty;
}

public class SearchCommand : AsyncCommand<SearchSettings>
{
    private readonly IApprenticeService _service;

    public SearchCommand(IApprenticeService service)
    {
        _service = service;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, SearchSettings settings)
    {
        IReadOnlyList<Models.Apprentice> results = [];

        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .StartAsync("[blue]Searching...[/]", async _ =>
            {
                results = await _service.SearchAsync(settings.Term);
            });

        if (results.Count == 0)
        {
            AnsiConsole.MarkupLine($"[yellow]No apprentices found matching '[bold]{Markup.Escape(settings.Term)}[/]'.[/]");
            return 0;
        }

        AnsiConsole.MarkupLine($"[blue]Found [bold]{results.Count}[/] apprentice(s) matching '[bold]{Markup.Escape(settings.Term)}[/]'.[/]");
        TableRenderer.RenderApprentices(results);
        return 0;
    }
}
