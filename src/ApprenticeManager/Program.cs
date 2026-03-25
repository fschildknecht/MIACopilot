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

// Configure Spectre.Console.Cli
var registrar = new TypeRegistrar(services);
var app = new CommandApp<DefaultCommand>(registrar);

app.Configure(config =>
{
    config.SetApplicationName("apprentice-manager");

    config.AddCommand<ListCommand>("list")
        .WithDescription("Show all apprentices in a formatted table.");

    config.AddCommand<AddCommand>("add")
        .WithDescription("Add a new apprentice using interactive prompts.");

    config.AddCommand<EditCommand>("edit")
        .WithDescription("Edit an existing apprentice by ID.");

    config.AddCommand<DeleteCommand>("delete")
        .WithDescription("Delete an apprentice by ID.");

    config.AddCommand<SearchCommand>("search")
        .WithDescription("Search apprentices by name or company.");
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
