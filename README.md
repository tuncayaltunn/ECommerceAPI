# ECommerceAPI

> An e-commerce backend API built on Onion Architecture, CQRS, and role-based dynamic authorization principles. Developed with ASP.NET Core 7, Entity Framework Core, and PostgreSQL.

This project was developed under the guidance of an instructor as part of a bootcamp training program. The goal is to gain hands-on, end-to-end experience with modern .NET ecosystem concepts including clean architecture, CQRS, authentication (JWT + Refresh Token), dynamic authorization, file management, email delivery, structured logging, and real-time communication.

---

## Table of Contents

- [Architecture](#architecture)
- [Technologies Used](#technologies-used)
- [Project Structure](#project-structure)
- [Features](#features)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Database & Migrations](#database--migrations)
- [API Endpoints](#api-endpoints)
- [Authorization Model](#authorization-model)
- [Logging](#logging)
- [Tests](#tests)
- [Known Limitations](#known-limitations)

---

## Architecture

The project is designed according to **Onion Architecture (Clean Architecture)** principles. Dependencies flow only from the outside in; inner layers have no knowledge of outer layers.

```
┌──────────────────────────────────────────────────────────┐
│                      Presentation                         │
│                   ECommerceAPI.API                        │
│   (Controllers, Filters, Middlewares, Program.cs)         │
└────────────────────────────┬─────────────────────────────┘
                             │
┌────────────────────────────┴─────────────────────────────┐
│                    Infrastructure                         │
│  ┌──────────────────┐ ┌──────────────┐ ┌──────────────┐  │
│  │   Persistence    │ │Infrastructure│ │   SignalR    │  │
│  │ (EF Core, Repos) │ │ (JWT, Mail,  │ │   (Hubs)     │  │
│  │                  │ │   Storage)   │ │              │  │
│  └──────────────────┘ └──────────────┘ └──────────────┘  │
└────────────────────────────┬─────────────────────────────┘
                             │
┌────────────────────────────┴─────────────────────────────┐
│                         Core                              │
│  ┌──────────────────────┐  ┌────────────────────────┐    │
│  │     Application      │  │        Domain          │    │
│  │ CQRS (MediatR), DTOs │  │   Entities, Common     │    │
│  │ Validators, Abstr.   │  │                        │    │
│  └──────────────────────┘  └────────────────────────┘    │
└──────────────────────────────────────────────────────────┘
```

**Design decisions:**

- **CQRS + MediatR**: Command and Query operations are clearly separated. Each use case is represented by its own `Request`, `Response`, and `Handler` trio.
- **Repository Pattern**: Generic data access via `IReadRepository<T>` and `IWriteRepository<T>`. Custom repositories per domain entity are added when needed.
- **Dependency Inversion**: All external services (Storage, Token, Mail) have interfaces defined in the Application layer, with implementations provided in the Infrastructure layer.
- **Dynamic Endpoint Authorization**: Actions marked with `AuthorizeDefinitionAttribute` are controlled at runtime via `RolePermissionFilter` against the role-endpoint mapping stored in the database.

---

## Technologies Used

| Layer / Concern        | Technology                                                      |
|------------------------|-----------------------------------------------------------------|
| Runtime                | .NET 7                                                          |
| Web Framework          | ASP.NET Core 7 (Web API)                                        |
| ORM                    | Entity Framework Core 7                                         |
| Database               | PostgreSQL (Npgsql provider)                                    |
| CQRS / Mediator        | MediatR 8                                                       |
| Validation             | FluentValidation 11                                             |
| Identity Management    | ASP.NET Core Identity                                           |
| Token                  | JWT Bearer (HS256) + Refresh Token                              |
| Real-Time              | SignalR                                                         |
| Logging                | Serilog (File + PostgreSQL sink)                                |
| API Documentation      | Swashbuckle (Swagger / OpenAPI)                                 |
| Mail                   | System.Net.Mail (SMTP)                                          |

---

## Project Structure

```
ECommerceAPI/
├── Core/
│   ├── ECommerceAPI.Domain/              # Entities, Identity entities, base classes
│   └── ECommerceAPI.Application/         # CQRS features, abstractions, DTOs, validators
│
├── Infrastructure/
│   ├── ECommerceAPI.Persistence/         # DbContext, repository implementations, migrations, services
│   ├── ECommerceAPI.Infrastructure/      # JWT, Mail, Local Storage, external services
│   └── ECommerceAPI.SignalR/             # Real-time hubs
│
├── Presentation/
│   └── ECommerceAPI.API/                 # Controllers, filters, middleware, Program.cs, appsettings
│
└── tests/
    └── ECommerceAPI.Application.Tests/   # Unit tests with xUnit + Moq + FluentAssertions
```

---

## Features

- **Product Management**: Product CRUD, pagination, multiple image upload, showcase image selection.
- **Basket**: Add items to basket, update quantity, remove items, list basket contents.
- **Order Management**: Create orders, list orders, order detail, complete order (with email notification).
- **Authentication**: User registration via ASP.NET Core Identity; JWT + Refresh Token flow; password reset via email.
- **Role & Permission Management**: Roles, assigning roles to users, dynamic endpoint-based authorization.
- **File Storage**: Abstract `IStorage` layer; currently implemented with `LocalStorage`. Placeholder ready for Azure / AWS.
- **Mail Service**: Password reset and order completion notifications.
- **Logging**: Structured logging with Serilog; logs are written simultaneously to a rolling file and the PostgreSQL `Logs` table. Each log entry is enriched with the current `user_name`.
- **Global Exception Handling**: All unhandled exceptions are logged and returned in a standard JSON format via a single middleware.
- **Swagger UI**: Automatically enabled in the development environment.

---

## Getting Started

### Prerequisites

- [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- [PostgreSQL 14+](https://www.postgresql.org/download/)
- (Optional) [Visual Studio 2022](https://visualstudio.microsoft.com/) or [JetBrains Rider](https://www.jetbrains.com/rider/)

### Clone & Setup

```bash
git clone https://github.com/<username>/ECommerceAPI.git
cd ECommerceAPI
```

### Create the Configuration File

`appsettings.json` is included in `.gitignore`; actual sensitive values are never committed to the repository. Before running the application, copy the example file and fill in your own values:

```bash
cp Presentation/ECommerceAPI.API/appsettings.Example.json Presentation/ECommerceAPI.API/appsettings.json
```

Then update the following fields in `Presentation/ECommerceAPI.API/appsettings.json`:

- `ConnectionStrings:PostgreSQL` — your local PostgreSQL connection string
- `Token:SecurityKey` — a cryptographically strong random string of at least 32 characters
- `Token:Audience`, `Token:Issuer` — values you define
- `Mail:Username`, `Mail:Password`, `Mail:Host` — your SMTP credentials (can be left empty if not used)

> 💡 **Production recommendation**: Store sensitive values in [.NET User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets), environment variables, or a secret store such as Azure Key Vault instead of `appsettings.json`.

### Restore, Build & Run

```bash
dotnet restore
dotnet build
dotnet run --project Presentation/ECommerceAPI.API
```

The API starts at `https://localhost:7285` by default. Swagger UI: `https://localhost:7285/swagger`

---

## Configuration

### Environment Variables

The `DesignTimeDbContextFactory` in the Persistence layer resolves the connection string for EF migration commands in the following order:

1. `ECOMMERCEAPI_CONNECTION_STRING` environment variable (if set)
2. `ConnectionStrings:PostgreSQL` in `Presentation/ECommerceAPI.API/appsettings.json`

Using option 1 is recommended for CI/CD environments.

### CORS

`Program.cs` only allows requests from `http://localhost:4200` and `https://localhost:4200` (Angular client) by default. If you are using a different frontend, you will need to update the CORS policy there.

---

## Database & Migrations

EF Core migrations are located under `Infrastructure/ECommerceAPI.Persistence/Migrations/`.

To add a new migration:

```bash
dotnet ef migrations add <MigrationName> \
  --project Infrastructure/ECommerceAPI.Persistence \
  --startup-project Presentation/ECommerceAPI.API
```

To update the database:

```bash
dotnet ef database update \
  --project Infrastructure/ECommerceAPI.Persistence \
  --startup-project Presentation/ECommerceAPI.API
```

---

## API Endpoints

> For the full interactive reference, see Swagger UI at `/swagger`. Key endpoints are summarized below.

### Auth (`/api/auth`)
| Method | Endpoint              | Description                               |
|--------|-----------------------|-------------------------------------------|
| POST   | `/login`              | User login (returns JWT)                  |
| POST   | `/refreshTokenLogin`  | Obtain new access token via refresh token |
| POST   | `/password-reset`     | Triggers password reset email             |
| POST   | `/verify-reset-token` | Validates password reset token            |

### Products (`/api/products`)
| Method | Endpoint                   | Authorization        |
|--------|----------------------------|----------------------|
| GET    | `/`                        | Public               |
| GET    | `/{id}`                    | Public               |
| POST   | `/`                        | Admin + Writing      |
| PUT    | `/`                        | Admin + Updating     |
| DELETE | `/{id}`                    | Admin + Deleting     |
| POST   | `/upload`                  | Admin + Writing      |
| GET    | `/getProductImages/{id}`   | Admin + Reading      |
| GET    | `/changeShowcaseImage`     | Admin + Updating     |
| GET    | `/deleteProductImage/{id}` | Admin + Deleting     |

### Baskets (`/api/baskets`)
Add, list, update quantity, and remove basket items.

### Orders (`/api/orders`)
Create, list, detail, and complete orders.

### Users (`/api/users`)
Create users, update passwords, list users, assign roles to users.

### Roles (`/api/roles`)
Role CRUD operations.

### AuthorizationEndpoints (`/api/authorizationendpoints`)
Assign roles to endpoints; query the current roles of an endpoint.

---

## Authorization Model

This project uses **database-driven dynamic authorization** instead of the conventional `[Authorize(Roles = "...")]` approach:

1. Action methods are decorated with `[AuthorizeDefinition(Menu, Definition, ActionType)]`.
2. On startup, these attributes are scanned via reflection and registered in the `Endpoints` table (via `AuthorizationEndpointService`).
3. An admin user defines which roles can call which endpoints through `AuthorizationEndpointsController`.
4. Every request passes through `RolePermissionFilter`; if the user's roles intersect with the endpoint's permitted roles, the request proceeds — otherwise `401` is returned.

This design allows permission management to live in the database rather than in code, making it updatable at runtime via an admin panel.

---

## Logging

- **Sinks**: `logs/log.txt` (rolling file) and PostgreSQL `Logs` table (`needAutoCreateTable: true`).
- **Custom column writers**: `RenderedMessageColumnWriter`, `MessageTemplateColumnWriter`, `LevelColumnWriter`, `TimestampColumnWriter`, `ExceptionColumnWriter`, `LogEventSerializedColumnWriter`, `UsernameColumnWriter`.
- **Enrichment**: The current user's `user_name` is pushed to each log entry via Serilog `LogContext`.
- **Request logging**: Every HTTP request is automatically logged via `app.UseSerilogRequestLogging()`.

---

## Tests

Unit tests for the Application layer are located under `tests/ECommerceAPI.Application.Tests/`. The testing strategy is to **validate pure business logic through mockable dependencies**, avoiding integration test complexity.

### Tools Used

| Package                | Purpose                          |
|------------------------|----------------------------------|
| **xUnit**              | Test framework                   |
| **Moq**                | Mocking dependencies             |
| **FluentAssertions**   | Readable assertion DSL           |
| **coverlet.collector** | Code coverage collection         |

### Current Test Coverage

| Test Class                         | Target                                                                         | # Tests |
|------------------------------------|--------------------------------------------------------------------------------|---------|
| `CreateProductCommandHandlerTests` | `CreateProductCommandHandler` (handler orchestration, mock verify, call order) | 3       |
| `CreateUserCommandHandlerTests`    | `CreateUserCommandHandler` (DTO mapping, success/failure flows)                | 2       |
| `CreateProductValidatorTests`      | `CreateProductValidator` (FluentValidation rules, edge cases)                  | 9       |

### Running Tests

```bash
# Run all tests
dotnet test

# Run only this project
dotnet test tests/ECommerceAPI.Application.Tests

# With code coverage
dotnet test --collect:"XPlat Code Coverage"
```

Visual Studio's Test Explorer discovers tests automatically.

> **Note**: The test project targets `net7.0`, but thanks to the `<RollForward>Major</RollForward>` setting, it also runs on newer .NET runtimes (8/9/10) on machines that do not have .NET 7 installed.

---

## Known Limitations

Since this project was developed for educational purposes, some areas have been intentionally left as to-dos:

- **External Authentication** (Google/Facebook) is defined at the interface level but has no implementation (`NotImplementedException`).
- **SignalR Hubs** (`OrderHub`, `ProductHub`) are scaffolded as skeletons; the real-time notification flow has not been wired up end-to-end.
- **Cloud Storage** (Azure Blob / AWS S3) has interfaces and enum values defined, but the implementation is `LocalStorage` only.
- **Unit / Integration test** coverage is still limited.
- **API versioning** is not implemented.
