# MyAppTemplate

A modern ASP.NET Core 9.0 web application starter template featuring role-based access control, user authentication, real-time monitoring, and a clean three-tier architecture.

## Features

- **User Authentication** — Secure login with BCrypt password hashing, 14-day cookie sessions, and JWT support for APIs
- **Role-Based Access Control (RBAC)** — Fine-grained permissions at both role and user levels
- **Module Management** — Hierarchical module system mapping to features and pages with dynamic menu generation
- **User Activity Logging** — Track all user actions with IP address, browser, and timestamp
- **Real-Time Monitoring** — SignalR integration for live system updates and notifications
- **Responsive Admin UI** — Bootstrap 5 with DataTables for data management and export capabilities
- **Structured Logging** — Serilog-based logging with daily rotation and retention policies

## Tech Stack

### Backend
- **.NET 9.0** with C# 13+
- **ASP.NET Core** MVC and API controllers
- **Entity Framework Core 9.0** (SQL Server provider)
- **AutoMapper 13.0.1** for DTO/entity mapping
- **BCrypt.Net-Next 4.1.0** for password hashing
- **Serilog 10.0.0** for structured logging
- **SignalR 1.2.9** for real-time communication

### Frontend
- **Bootstrap 5.3.3** CSS framework
- **jQuery 3.7.1** for DOM manipulation
- **DataTables 2.1.8** with export functionality
- **SweetAlert2 11.10.1** for modal dialogs
- **Moment.js 2.30.1** for date formatting
- **Microsoft SignalR 10.0.0** client library

### Database
- **SQL Server** with code-first EF Core migrations

## Project Structure

```
MyAppTemplate/
├── src/
│   ├── MyAppTemplate.App/          # Presentation layer (MVC, API, Services)
│   │   ├── Controllers/            # MVC controllers
│   │   ├── ApiControllers/         # REST API endpoints
│   │   ├── Services/               # Business logic
│   │   ├── Views/                  # Razor templates
│   │   ├── ViewModels/             # View-specific models
│   │   ├── Hubs/                   # SignalR hubs
│   │   └── wwwroot/                # Static assets
│   ├── MyAppTemplate.Contract/     # Abstraction layer
│   │   ├── Interfaces/             # Service and repository contracts
│   │   ├── DTO/                    # Data transfer objects
│   │   ├── Models/                 # Request/response models
│   │   └── Enums/                  # Shared enumerations
│   └── MyAppTemplate.Data/         # Data access layer
│       ├── ContextModels/          # EF Core entities
│       ├── Repositories/           # Data repositories
│       ├── Migrations/             # EF Core migrations
│       └── Helpers/                # Seed data and utilities
└── MyAppTemplate.slnx              # Solution file
```

## Getting Started

### Prerequisites

- **.NET 9 SDK** ([download](https://dotnet.microsoft.com/download/dotnet/9.0))
- **SQL Server 2019+** (local or remote instance)
- **Visual Studio 2022** or **VS Code** with C# extension

### Setup

1. **Clone and navigate to the repository**
   ```bash
   cd MyAppTemplate
   ```

2. **Configure the database connection**
   - Create `src/MyAppTemplate.App/appsettings.Development.json`
   - Update the `DefaultConnection` with your SQL Server instance
   
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=dbMyAppTemplateDEV01;Trusted_Connection=true;"
     }
   }
   ```

3. **Create and seed the database**
   ```bash
   dotnet ef database update --project src/MyAppTemplate.Data --startup-project src/MyAppTemplate.App
   ```

4. **Run the application**
   ```bash
   cd src/MyAppTemplate.App
   dotnet run
   ```

   Navigate to `https://localhost:5001` (or the configured port)

5. **Default credentials**
   - **Username:** `admin`
   - **Password:** Check `SeedData.cs` for the default password

## Architecture

### Three-Tier Layering

```
MyAppTemplate.App (Presentation)
    ↓ references
MyAppTemplate.Contract (Abstractions/DTOs)
    ↓ references
MyAppTemplate.Data (Data Access)
```

- **Presentation Layer** (`MyAppTemplate.App`) — Controllers, views, service injection, and API endpoints
- **Contract Layer** (`MyAppTemplate.Contract`) — Interfaces, DTOs, enums, and shared models
- **Data Layer** (`MyAppTemplate.Data`) — Repositories, EF Core entities, and database migrations

### Key Patterns

- **Repository Pattern** — All data access through repositories inheriting from `BaseRepository<T>`
- **Service Layer** — Business logic isolated in services behind interfaces
- **Dependency Injection** — Constructor-based DI for services and repositories
- **AutoMapper** — Automatic DTO↔Entity mapping via profiles
- **Async/Await** — All I/O operations are async-first

## Development Guidelines

### Code Standards

- **Namespaces** — File-scoped namespaces following folder structure
- **Nullable reference types** — Always enabled
- **Async first** — Use `async Task<T>` for all I/O operations
- **No magic strings** — Use `appsettings.json` for configuration
- **Thin controllers** — Delegate business logic to services

### Naming Conventions

| Element | Convention | Example |
|---------|-----------|---------|
| Interface | `I` prefix | `IUserService` |
| Service | PascalCase | `AuthenticationService` |
| Repository | PascalCase + `Repository` | `UserRepository` |
| DTO | PascalCase + `Dto` | `UserDto` |
| ViewModel | PascalCase + `ViewModel` | `UserIndexViewModel` |
| Entity | Singular PascalCase | `User`, `Role` |
| Controller | PascalCase + `Controller` | `UserController` |

### Adding a New Feature

1. Create the entity in `MyAppTemplate.Data/ContextModels/`
2. Add repository in `MyAppTemplate.Data/Repositories/` (inherits `BaseRepository<T>`)
3. Add DTO in `MyAppTemplate.Contract/DTO/`
4. Add service interface in `MyAppTemplate.Contract/Interfaces/`
5. Implement service in `MyAppTemplate.App/Services/`
6. Add controller in `MyAppTemplate.App/Controllers/` or `ApiControllers/`
7. Register service in `ServiceRegistration.cs`
8. Create migration: `dotnet ef migrations add YourMigrationName`

### Database Migrations

```bash
dotnet ef migrations add AddUserTable \
  --project src/MyAppTemplate.Data \
  --startup-project src/MyAppTemplate.App

dotnet ef database update \
  --project src/MyAppTemplate.Data \
  --startup-project src/MyAppTemplate.App
```

**Never edit existing migration files.** Create a new migration to fix issues.

## Configuration

| Setting | Purpose |
|---------|---------|
| `ConnectionStrings.DefaultConnection` | SQL Server connection string |
| `JwtSettings` | JWT token configuration |
| `CookieSettings` | Cookie session configuration |
| `Serilog` | Logging configuration |

## Logging

Logs are written to `logs/` directory with daily rotation:

```json
{
  "Serilog": {
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "logs/MyAppTemplate-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 7
        }
      }
    ]
  }
}
```

## Troubleshooting

### Database Connection Issues
- Verify SQL Server is running
- Check connection string in `appsettings.Development.json`
- Verify Windows user has appropriate permissions

### Application Won't Start
- Clear `bin/` and `obj/` folders: `dotnet clean MyAppTemplate.slnx`
- Restore and rebuild: `dotnet restore` && `dotnet build`
- Check log files in `logs/` directory

## Security

- **Passwords** — Hashed with BCrypt
- **Sessions** — HttpOnly, Secure, SameSite=Strict cookies
- **SQL Injection** — Prevented by EF Core parameterized queries
- **CSRF** — ASP.NET Core antiforgery enabled
- **Secrets** — Use `appsettings.json` or Azure Key Vault

## Customization Notes

When using this template:

- Update `AdminEmail` domain in `appsettings.json`
- Update database names in connection strings
- Customize JWT issuer and audience
- Review and update `SeedData.cs` with your admin user
- Customize modules for your application
- Update this README with project-specific information

---

**Template Version:** April 2026  
**Built with:** .NET 9.0, ASP.NET Core, Entity Framework Core 9.0