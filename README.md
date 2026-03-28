# OpenAPI Guard

Prevent breaking API changes and enforce API standards — for .NET teams.

## What it does

- **Upload** OpenAPI v3 specs (JSON/YAML) per project
- **Compare** two spec versions and get a breaking-change report
- **Configure rules** (block removed endpoints, type changes, require auth, etc.)
- **Link a GitHub repo** (informational for now; CI integration coming)

## Tech stack

- ASP.NET Core 9 / Blazor Server (interactive SSR)
- EF Core 9 + PostgreSQL
- ASP.NET Core Identity (auth)
- `Microsoft.OpenApi.Readers` for spec parsing

## Quick start

### 1. Start PostgreSQL

```bash
docker compose up -d
```

### 2. Run the app

```bash
cd src/OpenApiGuard.App
dotnet run
```

Open **http://localhost:5000** (or the port shown in the console).

The database is auto-migrated on startup.

## Solution structure

| Project | Purpose |
|---------|---------|
| `OpenApiGuard.App` | Blazor Server host (UI + DI + migrations) |
| `OpenApiGuard.Api` | Web API project (future stand-alone API) |
| `OpenApiGuard.Core` | Domain entities + diff engine |
| `OpenApiGuard.Contracts` | DTOs shared by UI and API |
| `OpenApiGuard.Infrastructure` | EF Core, DbContext, Identity |

## License

MIT