# OpenAPI Guard

## Project Structure
- OpenApiGuard.App: Blazor Server Application
- OpenApiGuard.Api: Web API
- OpenApiGuard.Core: Core business logic and entities
- OpenApiGuard.Contracts: DTOs and contracts
- OpenApiGuard.Infrastructure: Data access and services

## Run Instructions
1. Ensure Docker is running.
2. Run `docker-compose up` to start PostgreSQL database.
3. Open a terminal and navigate to the root of the solution.
4. Run `dotnet build` to build the solution.
5. Run `dotnet run --project OpenApiGuard.App` to start the Blazor server application.