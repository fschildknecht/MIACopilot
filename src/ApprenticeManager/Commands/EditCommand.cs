using ApprenticeManager.Services;
using ApprenticeManager.Validators;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace ApprenticeManager.Commands;

public class EditSettings : CommandSettings
{
    [CommandArgument(0, "<id>")]
    [Description("The ID of the apprentice to edit.")]
    public int Id { get; set; }
}

public class EditCommand : AsyncCommand<EditSettings>
{
    private readonly IApprenticeService _service;
    private readonly ApprenticeValidator _validator;

    public EditCommand(IApprenticeService service, ApprenticeValidator validator)
    {
        _service = service;
        _validator = validator;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, EditSettings settings)
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

        AnsiConsole.MarkupLine($"[blue]Editing apprentice: [bold]{Markup.Escape(apprentice.FullName)}[/][/]");
        AnsiConsole.MarkupLine("[yellow]Press Enter to keep the current value.[/]");
        AnsiConsole.WriteLine();

        apprentice = AddCommand.PromptForApprentice(apprentice);

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

        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .StartAsync("[blue]Saving changes...[/]", async _ =>
            {
                await _service.UpdateAsync(apprentice);
            });

        AnsiConsole.MarkupLine($"[green]Apprentice [bold]{Markup.Escape(apprentice.FullName)}[/] updated successfully.[/]");
        return 0;
    }
}
