# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["OpenApiGuard.sln", "."]
COPY ["src/OpenApiGuard.Core/OpenApiGuard.Core.csproj", "src/OpenApiGuard.Core/"]
COPY ["src/OpenApiGuard.Contracts/OpenApiGuard.Contracts.csproj", "src/OpenApiGuard.Contracts/"]
COPY ["src/OpenApiGuard.Infrastructure/OpenApiGuard.Infrastructure.csproj", "src/OpenApiGuard.Infrastructure/"]
COPY ["src/OpenApiGuard.Api/OpenApiGuard.Api.csproj", "src/OpenApiGuard.Api/"]
COPY ["src/OpenApiGuard.App/OpenApiGuard.App.csproj", "src/OpenApiGuard.App/"]

# Restore dependencies
RUN dotnet restore

# Copy source code
COPY . .

# Build the application
RUN dotnet build -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "src/OpenApiGuard.App/OpenApiGuard.App.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose port
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

# Run the application
ENTRYPOINT ["dotnet", "OpenApiGuard.App.dll"]
