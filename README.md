# account-service
This is an account service API in C#

## Upgrading to .NET 10

Follow these steps to upgrade the project from .NET 9 to .NET 10:

### 1. Update the Target Framework in `account-service.csproj`
Change:
```xml
<TargetFramework>net9.0</TargetFramework>
```
To:
```xml
<TargetFramework>net10.0</TargetFramework>
```

### 2. Update Package Versions in `account-service.csproj`
Bump all Microsoft packages from `9.0.x` to `10.0.x`:
```xml
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="10.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="10.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.0" />
```
> Leave `Swashbuckle.AspNetCore` as-is until a .NET 10 compatible version is available.

### 3. Restore & Build
```bash
dotnet restore
dotnet build
```

### 4. Update the EF Core CLI Tool (if installed globally)
```bash
dotnet tool update --global dotnet-ef
```

---

## Entity Framework Core

This project uses **EF Core** with **SQLite** by default. The database file (`accounts.db`) is created automatically in the project root.

### Prerequisites — Install the EF Core CLI tool
```bash
dotnet tool install --global dotnet-ef
```
Verify it's installed:
```bash
dotnet ef --version
```

### Connection String
Configured in `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=accounts.db"
}
```
To switch databases, update `ServiceCollectionExtensions.cs` with the appropriate provider and update the connection string here.

### Creating a Migration
Run this after making any changes to your EF entity models:
```bash
dotnet ef migrations add <MigrationName>
```
Example:
```bash
dotnet ef migrations add InitialCreate
```
Migration files are generated in the `Migrations/` folder — **commit these to source control**.

### Applying Migrations to the Database
```bash
dotnet ef database update
```
This creates the database (if it doesn't exist) and applies all pending migrations.

### Reverting a Migration
Revert to a specific migration by name:
```bash
dotnet ef database update <PreviousMigrationName>
```
Or revert all migrations (empty database):
```bash
dotnet ef database update 0
```

### Removing the Last Migration
If you haven't applied the migration to the database yet:
```bash
dotnet ef migrations remove
```

### Listing Migrations
```bash
dotnet ef migrations list
```

### Switching to a Different Database Provider
1. Replace the NuGet package in `account-service.csproj`:
   - PostgreSQL: `Npgsql.EntityFrameworkCore.PostgreSQL`
   - SQL Server: `Microsoft.EntityFrameworkCore.SqlServer`
2. Update `UseSqlite(...)` in `Infrastructure/Persistence/ServiceCollectionExtensions.cs` to `UseNpgsql(...)` or `UseSqlServer(...)`
3. Update the connection string in `appsettings.json`
4. Delete existing migrations and recreate them:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

