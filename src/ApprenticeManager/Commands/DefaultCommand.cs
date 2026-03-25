using ApprenticeManager.Services;
using ApprenticeManager.UI;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ApprenticeManager.Commands;

public class DefaultCommand : AsyncCommand
{
    private readonly IApprenticeService _service;
    private readonly ApprenticeManager.Validators.ApprenticeValidator _validator;

    public DefaultCommand(IApprenticeService service, ApprenticeManager.Validators.ApprenticeValidator validator)
    {
        _service = service;
        _validator = validator;
    }

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        TableRenderer.PrintBanner();

        while (true)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[blue]What would you like to do?[/]")
                    .AddChoices(
                        "List all apprentices",
                        "Add apprentice",
                        "Edit apprentice",
                        "Delete apprentice",
                        "Search apprentices",
                        "Exit"));

            switch (choice)
            {
                case "List all apprentices":
                    await new ListCommand(_service).ExecuteAsync(context);
                    break;

                case "Add apprentice":
                    await new AddCommand(_service, _validator).ExecuteAsync(context);
                    break;

                case "Edit apprentice":
                    var editId = AnsiConsole.Ask<int>("[yellow]Enter apprentice ID to edit:[/]");
                    var editSettings = new EditSettings { Id = editId };
                    await new EditCommand(_service, _validator).ExecuteAsync(context, editSettings);
                    break;

                case "Delete apprentice":
                    var deleteId = AnsiConsole.Ask<int>("[yellow]Enter apprentice ID to delete:[/]");
                    var deleteSettings = new DeleteSettings { Id = deleteId };
                    await new DeleteCommand(_service).ExecuteAsync(context, deleteSettings);
                    break;

                case "Search apprentices":
                    var term = AnsiConsole.Ask<string>("[yellow]Enter search term:[/]");
                    var searchSettings = new SearchSettings { Term = term };
                    await new SearchCommand(_service).ExecuteAsync(context, searchSettings);
                    break;

                case "Exit":
                    AnsiConsole.MarkupLine("[blue]Goodbye![/]");
                    return 0;
            }

            AnsiConsole.WriteLine();
        }
    }
}
