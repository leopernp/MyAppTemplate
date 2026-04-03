# MyAppTemplate

A modern ASP.NET Core 9.0 web application for file system and administrative management with role-based access control, user authentication, and real-time system monitoring.

## Features

- **User Authentication** — Secure login with BCrypt password hashing, 14-day cookie sessions, and JWT support for APIs
- **Role-Based Access Control (RBAC)** — Fine-grained permissions at both role and user levels
- **Module Management** — Hierarchical module system mapping to features and pages with dynamic menu generation
- **User Activity Logging** — Track all user actions with IP address, browser, and timestamp
- **Real-Time Monitoring** — SignalR integration for live system updates
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
│   │   ├── Controllers/        # MVC controllers
│   │   ├── ApiControllers/     # REST API endpoints
│   │   ├── Services/           # Business logic
│   │   ├── Views/              # Razor templates
│   │   ├── ViewModels/         # View-specific models
│   │   ├── Hubs/               # SignalR hubs
│   │   └── wwwroot/            # Static assets
│   ├── MyAppTemplate.Data/         # Data access layer
│   │   ├── ContextModels/      # EF Core entities
│   │   ├── Repositories/       # Data repositories
│   │   ├── Migrations/         # EF Core migrations
│   │   └── Helpers/            # Seed data and utilities
│   └── MyAppTemplate.Contract/     # Abstraction layer
│       ├── Interfaces/         # Service and repository contracts
│       ├── DTO/                # Data transfer objects
│       ├── Models/             # Request/response models
│       └── Enums/              # Shared enumerations
└── MyAppTemplate.slnx              # Solution file
```

## Getting Started

### Prerequisites

- **.NET 9 SDK** ([download](https://dotnet.microsoft.com/download/dotnet/9.0))
- **SQL Server 2019+** (local or remote instance)
- **Visual Studio 2022** or **VS Code** with C# extension

### Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd MyAppTemplate
   ```

2. **Configure the database connection**
   - Open `src/MyAppTemplate.App/appsettings.Development.json`
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
   - **Password:** Check `SeedData.cs` for the default admin password

### Configuration

All settings are in `appsettings.json` and `appsettings.Development.json`:

| Setting | Purpose |
|---------|---------|
| `ConnectionStrings.DefaultConnection` | SQL Server connection string |
| `JwtSettings` | JWT token configuration (issuer, audience, expiration) |
| `CookieSettings` | Cookie session configuration |
| `Serilog` | Logging configuration and file paths |

## Architecture

### Layering

The solution follows a three-tier architecture:

```
MyAppTemplate.App (Presentation)
    ↓ references
MyAppTemplate.Contract (Abstractions/DTOs)
    ↓ references
MyAppTemplate.Data (Data Access)
```

- **Presentation Layer** (`MyAppTemplate.App`) — Controllers, views, and service injection
- **Contract Layer** (`MyAppTemplate.Contract`) — Interfaces, DTOs, enums, and shared models
- **Data Layer** (`MyAppTemplate.Data`) — Repositories, EF Core entities, and migrations

### Key Patterns

- **Repository Pattern** — All data access goes through repositories inheriting from `BaseRepository<T>`
- **Service Layer** — Business logic isolated in services behind interfaces
- **Dependency Injection** — Constructor-based DI for all services and repositories
- **AutoMapper** — Automatic DTO↔Entity mapping via profiles
- **Async/Await** — All I/O operations are async

### Authentication & Authorization

- **Web Auth** — Cookie-based with 14-day sliding expiration, HttpOnly + Secure flags
- **API Auth** — JWT Bearer tokens for stateless API calls
- **Permissions** — Dual-level control via `RoleModulePermission` (role-wide) and `UserModulePermission` (user-specific)
- **Account Lockout** — 3 failed login attempts triggers 5-minute lockout

## Development Guidelines

### Code Standards

- **Namespaces** — File-scoped namespaces following folder structure
- **Nullable reference types** — Always enabled; handle nullability explicitly
- **Async first** — Use `async Task<T>` for all I/O-bound operations
- **No magic strings** — Centralize configuration in `appsettings.json`
- **Thin controllers** — Delegate to services; controllers only validate input and return results
- **No data access in services** — Always delegate to repositories

### Naming Conventions

| Element | Convention | Example |
|---------|-----------|---------|
| Interface | `I` prefix | `IUserService` |
| Service | PascalCase, no suffix | `AuthenticationService` |
| Repository | PascalCase + `Repository` | `UserRepository` |
| DTO | PascalCase + `Dto` | `UserDto` |
| ViewModel | PascalCase + `ViewModel` | `UserIndexViewModel` |
| Entity | Singular PascalCase | `User`, `Role` |
| Controller | PascalCase + `Controller` | `UserController` |

### Adding a New Feature

1. **Create the entity** in `MyAppTemplate.Data/ContextModels/`
2. **Add repository** in `MyAppTemplate.Data/Repositories/` (inherits `BaseRepository<T>`)
3. **Add DTO** in `MyAppTemplate.Contract/DTO/`
4. **Add service interface** in `MyAppTemplate.Contract/Interfaces/`
5. **Implement service** in `MyAppTemplate.App/Services/`
6. **Add controller** in `MyAppTemplate.App/Controllers/` or `ApiControllers/`
7. **Register service** in `ServiceRegistration.cs`
8. **Create migration** — Run `dotnet ef migrations add YourMigrationName`
9. **Add tests** (if applicable)

### Database Migrations

Always create named migrations:

```bash
dotnet ef migrations add AddUserTable --project src/MyAppTemplate.Data --startup-project src/MyAppTemplate.App
dotnet ef database update --project src/MyAppTemplate.Data --startup-project src/MyAppTemplate.App
```

**Never edit existing migration files.** Create a new migration to fix issues.

### Running Tests

```bash
dotnet test MyAppTemplate.sln
```

## API Endpoints

Base URL: `/api/v1/`

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/auth/login` | User login (returns JWT token) |
| `POST` | `/auth/logout` | User logout |
| `GET` | `/users` | List all users (admin only) |
| `GET` | `/users/{id}` | Get user details |
| `POST` | `/users` | Create new user |
| `PUT` | `/users/{id}` | Update user |
| `DELETE` | `/users/{id}` | Delete user |

See your OpenAPI/Swagger documentation for the full list (if enabled).

## Logging

Logs are written to `logs/` directory with daily rotation:

```
logs/
├── MyAppTemplate-20260403.log
├── MyAppTemplate-20260402.log
└── MyAppTemplate-20260401.log
```

Configure retention and file size limits in `appsettings.json`:

```json
{
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "logs/MyAppTemplate-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 7,
          "fileSizeLimitBytes": 52428800
        }
      }
    ]
  }
}
```

## Troubleshooting

### Database Connection Issues

- Verify SQL Server is running: `sqlcmd -S YOUR_SERVER -E -Q "SELECT @@VERSION"`
- Check connection string in `appsettings.Development.json`
- Ensure your Windows user has appropriate SQL Server permissions

### Migration Failures

- Check for conflicting migrations: `dotnet ef migrations list`
- Verify the database context is up to date: `dotnet ef database update`
- Review recent commit changes to ensure all migrations are applied

### Application Won't Start

- Clear `bin/` and `obj/` folders
  ```bash
  dotnet clean MyAppTemplate.sln
  dotnet restore MyAppTemplate.sln
  dotnet build MyAppTemplate.sln
  ```
- Check Serilog configuration and log file permissions
- Review the application logs for error details

## Contributing

1. Create a feature branch from `master`
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. Follow the code standards outlined in [Code Standards](#code-standards)

3. Commit with clear, descriptive messages
   ```bash
   git commit -m "Feature: Add user profile management"
   ```

4. Push and create a pull request
   ```bash
   git push origin feature/your-feature-name
   ```

5. Ensure all tests pass and code review is complete before merging

## Security

- **Passwords** — Hashed with BCrypt; never stored in plain text
- **Sessions** — HttpOnly, Secure, and SameSite=Strict cookies
- **SQL Injection** — Prevented by EF Core parameterized queries
- **CSRF** — ASP.NET Core antiforgery tokens enabled by default
- **Secrets** — Never hardcode secrets; use `appsettings.json` or Azure Key Vault

## License

[Add your license here]

## Support

For issues, questions, or suggestions:
- Open an issue on GitHub
- Contact the development team
- Check the documentation in the `docs/` folder

---

**Last Updated:** April 2026  
**Maintainers:** [Your Team/Name]
