# Support Ticket Management

Full-stack Support Ticket Management system built for the AI-assisted practical assessment (Core scope).

## Technology Stack

| Layer | Technology |
|-------|------------|
| Frontend | React, TypeScript, Vite, npm |
| Backend | ASP.NET Core Web API, .NET 8, C# |
| ORM | Entity Framework Core 8 |
| Database | Microsoft SQL Server Express (`localhost\SQLEXPRESS`) |
| Database name | `SupportTicketDb` |
| Tests | xUnit + ASP.NET Core integration tests |
| API docs | Swagger/OpenAPI |

## Prerequisites

- .NET 8 SDK
- Node.js 18+ and npm
- SQL Server Express instance at `localhost\SQLEXPRESS`
- (Optional) SSMS for database inspection

## Project Structure

```
ai-practical-assessment/
├── src/
│   ├── frontend/support-ticket-ui/     # React UI
│   └── backend/
│       ├── SupportTicket.Api/
│       ├── SupportTicket.Application/
│       ├── SupportTicket.Domain/
│       └── SupportTicket.Infrastructure/
├── tests/SupportTicket.IntegrationTests/
├── database/
├── ai-prompts/
└── tool-specific/cursor-workflow/
```

## Database Configuration

Runtime connection string (Windows authentication):

```
Server=localhost\SQLEXPRESS;Database=SupportTicketDb;Trusted_Connection=True;TrustServerCertificate=True;
```

Configured in:

- `src/backend/SupportTicket.Api/appsettings.json`
- `src/backend/SupportTicket.Api/appsettings.Development.json`
- Example copy: `src/backend/SupportTicket.Api/appsettings.Example.json`

**Do not** point the app at `master`, LocalDB, SQLite, or other databases.

## How to Run EF Core Migrations

From `src/backend`:

```powershell
dotnet ef database update --project SupportTicket.Infrastructure --startup-project SupportTicket.Api
```

To create a new migration after schema changes:

```powershell
dotnet ef migrations add <Name> --project SupportTicket.Infrastructure --startup-project SupportTicket.Api --output-dir Persistence/Migrations
```

On API startup (non-Testing), the app applies pending migrations and runs idempotent seed data.

## Seed Data

Seeded automatically on API startup:

- 4 users (Alice, Bob, Carol, Dave)
- 5 sample tickets across all statuses
- Comments on active tickets

Seed is idempotent (safe to re-run).

## How to Run the Backend

```powershell
cd src/backend/SupportTicket.Api
dotnet run
```

- API: `http://localhost:5270`
- Swagger: `http://localhost:5270/swagger`

## How to Run the Frontend

```powershell
cd src/frontend/support-ticket-ui
npm install
npm run dev
```

- UI: `http://localhost:5173`
- Vite proxies `/api` to `http://localhost:5270`

## How to Run Tests

```powershell
dotnet test tests/SupportTicket.IntegrationTests/SupportTicket.IntegrationTests.csproj
```

Results are recorded in `test-results.md`.

## API Overview

| Method | Path | Purpose |
|--------|------|---------|
| GET | `/api/users` | List seeded users |
| GET | `/api/tickets?search=&status=` | List / search / filter |
| GET | `/api/tickets/{id}` | Ticket detail + comments |
| POST | `/api/tickets` | Create ticket |
| PUT | `/api/tickets/{id}` | Update fields (not status) |
| POST | `/api/tickets/{id}/status` | Status transition |
| GET | `/api/tickets/{id}/valid-transitions` | Allowed next statuses |
| POST | `/api/tickets/{ticketId}/comments` | Add comment |

Full contract: `api-contract.md`

## Status State Machine

Valid transitions only:

- Open → In Progress
- In Progress → Resolved
- Resolved → Closed
- Open → Cancelled
- In Progress → Cancelled

Invalid transitions are rejected by the backend.

## Known Limitations (Core)

- No authentication / authorization
- No user CRUD UI
- No pagination / sorting beyond default UpdatedAt desc
- No Stretch features

## AI-Assisted Development Workflow

Developed in Cursor Agent mode with phased delivery:

1. Requirements & acceptance criteria
2. Architecture / API / data model design
3. Project setup
4. Database + migrations + seed
5. Backend APIs + state machine
6. Integration tests
7. React UI
8. Verification & documentation

Prompt history lives under `ai-prompts/`.
