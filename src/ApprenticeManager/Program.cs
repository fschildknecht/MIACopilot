using ApprenticeManager;
using ApprenticeManager.Commands;
using ApprenticeManager.Data;
using ApprenticeManager.Services;
using ApprenticeManager.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;

// DI container setup
var services = new ServiceCollection();

services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(AppDbContext.ConnectionString));

services.AddScoped<IApprenticeService, ApprenticeService>();
services.AddScoped<IGradeService, GradeService>();
services.AddScoped<ILearningJournalService, LearningJournalService>();
services.AddScoped<IVocationalTrainerService, VocationalTrainerService>();
services.AddScoped<ICompanyService, CompanyService>();
services.AddSingleton<AppState>();
services.AddScoped<ApprenticeValidator>();
services.AddScoped<ListCommand>();
services.AddScoped<AddCommand>();
services.AddScoped<EditCommand>();
services.AddScoped<DeleteCommand>();
services.AddScoped<SearchCommand>();
services.AddScoped<DefaultCommand>();

var serviceProvider = services.BuildServiceProvider();

// Apply EF Core migrations on startup
using (var scope = serviceProvider.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

// Mode selection
AnsiConsole.Write(new FigletText("Apprentice Manager").LeftJustified().Color(Color.Blue));

var mode = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .Title("[blue]Wie möchten Sie die Anwendung öffnen?[/]")
        .AddChoices("Konsole", "App öffnen"));

if (mode == "App öffnen")
{
    var wpfThread = new System.Threading.Thread(() =>
    {
        var mainWindow = new ApprenticeManager.UI.App.MainWindow(serviceProvider);
        var wpfApp = new System.Windows.Application();
        wpfApp.ShutdownMode = System.Windows.ShutdownMode.OnMainWindowClose;
        wpfApp.Run(mainWindow);
    });
    wpfThread.SetApartmentState(System.Threading.ApartmentState.STA);
    wpfThread.Start();
    wpfThread.Join();
    return 0;
}

// Configure Spectre.Console.Cli
var registrar = new TypeRegistrar(services);
var app = new CommandApp<DefaultCommand>(registrar);

app.Configure(config =>
{
    config.SetApplicationName("apprentice-manager");

    config.AddCommand<ListCommand>("list")
        .WithDescription("Alle Lernenden in einer Tabelle anzeigen.");

    config.AddCommand<AddCommand>("add")
        .WithDescription("Neuen Lernenden über interaktive Eingabe hinzufügen.");

    config.AddCommand<EditCommand>("edit")
        .WithDescription("Lernenden nach ID bearbeiten.");

    config.AddCommand<DeleteCommand>("delete")
        .WithDescription("Lernenden nach ID löschen.");

    config.AddCommand<SearchCommand>("search")
        .WithDescription("Lernende nach Name oder Betrieb suchen.");
});

try
{
    return await app.RunAsync(args);
}
catch (Exception ex)
{
    AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
    return 1;
}
