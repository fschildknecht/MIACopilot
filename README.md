# MIACopilot – Apprentice Manager (Lehrlingsverwaltung)

A .NET 10 console application for managing apprentices, built with Spectre.Console, Entity Framework Core 9 (SQLite), FluentValidation, and Microsoft.Extensions.DependencyInjection.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [EF Core CLI tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet):
  ```bash
  dotnet tool install --global dotnet-ef
  ```

## Project Structure

```
ApprenticeManager.sln
├── src/
│   └── ApprenticeManager/          # Console application
│       ├── Commands/               # CLI commands (list, add, edit, delete, search, default)
│       ├── Data/                   # AppDbContext + EF Core Migrations
│       ├── Models/                 # Apprentice entity
│       ├── Services/               # IApprenticeService + ApprenticeService
│       ├── Validators/             # FluentValidation validators
│       ├── UI/                     # Spectre.Console rendering helpers
│       ├── TypeRegistrar.cs        # DI wiring for Spectre.Console.Cli
│       └── Program.cs
└── tests/
    └── ApprenticeManager.Tests/    # xUnit tests (service + validator)
```

## Setup & Run

1. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

2. **Build:**
   ```bash
   dotnet build
   ```

3. **Run the application** (interactive menu):
   ```bash
   dotnet run --project src/ApprenticeManager
   ```

   The database file `apprentices.db` is created automatically next to the executable on first run via EF Core migrations.

## CLI Commands

| Command              | Description                                |
|----------------------|--------------------------------------------|
| *(no args)*          | Interactive main menu (SelectionPrompt)    |
| `list`               | Show all apprentices in a formatted table  |
| `add`                | Add a new apprentice via interactive prompts |
| `edit <id>`          | Edit an existing apprentice by ID          |
| `delete <id>`        | Delete an apprentice by ID (with confirmation) |
| `search <term>`      | Search by name, company, or occupation     |

**Examples:**
```bash
dotnet run --project src/ApprenticeManager -- list
dotnet run --project src/ApprenticeManager -- add
dotnet run --project src/ApprenticeManager -- edit 1
dotnet run --project src/ApprenticeManager -- delete 2
dotnet run --project src/ApprenticeManager -- search "ACME"
```

## Database Migrations

Migrations are applied automatically on startup. To create a new migration manually:
```bash
cd src/ApprenticeManager
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

## Running Tests

```bash
dotnet test
```

Tests use the EF Core InMemory provider and cover:
- `ApprenticeService` CRUD operations
- `ApprenticeValidator` with valid and invalid inputs

## Tech Stack

| Package | Version |
|---|---|
| .NET | 10 |
| Spectre.Console + Spectre.Console.Cli | 0.49.1 |
| Entity Framework Core (SQLite) | 9.0.3 |
| Microsoft.Extensions.DependencyInjection | 10.0.0 |
| FluentValidation | 11.11.0 |
| xUnit | (latest via template) |
| FluentAssertions | 7.0.0 |
