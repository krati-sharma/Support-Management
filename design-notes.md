# Design Notes

## Architecture Overview

### Three-tier full-stack

1. **Presentation:** React SPA (Vite dev server, static build for production)
2. **Application/API:** ASP.NET Core Web API (.NET 8) with layered architecture
3. **Data:** SQL Server Express via EF Core 8

### Backend Layering

```
SupportTicket.Api
    ├── Controllers (HTTP only)
    ├── Middleware (exception handling, CORS)
    └── Program.cs (DI composition)

SupportTicket.Application
    ├── Services (TicketService, CommentService, UserQueryService)
    ├── StatusTransitionService ← state machine (critical)
    ├── DTOs (request/response)
    ├── Validators
    └── Interfaces (ITicketService, etc.)

SupportTicket.Domain
    ├── Entities (User, Ticket, Comment)
    ├── Enums (TicketStatus, Priority, UserRole)
    └── Exceptions (InvalidStatusTransitionException, NotFoundException)

SupportTicket.Infrastructure
    ├── AppDbContext
    ├── Entity configurations (Fluent API)
    ├── Migrations
    └── DbInitializer / SeedData
```

**Principle:** Controllers delegate to application services. EF entities are not returned directly from API — use DTOs.

---

## Frontend Design

### Structure

```
src/frontend/support-ticket-ui/
├── src/
│   ├── api/
│   │   ├── client.ts          # fetch wrapper, error parsing
│   │   └── tickets.ts         # API functions
│   ├── types/
│   │   └── index.ts           # Ticket, Comment, User, enums
│   ├── pages/
│   │   ├── TicketListPage.tsx
│   │   ├── CreateTicketPage.tsx
│   │   └── TicketDetailPage.tsx
│   ├── components/
│   │   ├── TicketForm.tsx
│   │   ├── StatusTransition.tsx
│   │   ├── CommentList.tsx
│   │   ├── CommentForm.tsx
│   │   ├── SearchBar.tsx
│   │   ├── StatusFilter.tsx
│   │   ├── UserSelect.tsx
│   │   ├── LoadingSpinner.tsx
│   │   ├── EmptyState.tsx
│   │   └── ErrorAlert.tsx
│   ├── App.tsx                # React Router routes
│   └── main.tsx
├── vite.config.ts             # proxy /api → backend
└── package.json
```

### Routing

| Route | Page | Purpose |
|-------|------|---------|
| `/` | TicketListPage | List, search, filter |
| `/tickets/new` | CreateTicketPage | Create ticket |
| `/tickets/:id` | TicketDetailPage | View, edit, status, comments |

### State Management

- **No Redux/Zustand** — local `useState` + `useEffect` sufficient for Core scope.
- API calls via typed fetch functions.
- Loading/error state per page.

### Status Transition UX

- Query valid next statuses from backend (`GET /api/tickets/{id}/valid-transitions`) **or** derive client-side from current status for display only.
- **Backend remains authoritative** — UI may disable invalid options but must handle API rejection.
- On invalid transition attempt, display API error message.

---

## Backend Design

### Controllers (thin)

| Controller | Endpoints |
|------------|-----------|
| `TicketsController` | CRUD list/detail/create/update, status change, valid transitions |
| `CommentsController` | Add comment to ticket |
| `UsersController` | List seeded users (read-only) |

### StatusTransitionService (critical)

Centralized, testable component:

```csharp
// Conceptual design
public class StatusTransitionService
{
    private static readonly Dictionary<TicketStatus, HashSet<TicketStatus>> Allowed =
        new()
        {
            [TicketStatus.Open] = { InProgress, Cancelled },
            [TicketStatus.InProgress] = { Resolved, Cancelled },
            [TicketStatus.Resolved] = { Closed },
            [TicketStatus.Closed] = { },
            [TicketStatus.Cancelled] = { },
        };

    public bool CanTransition(TicketStatus from, TicketStatus to) => ...
    public void ValidateTransition(TicketStatus from, TicketStatus to) // throws
}
```

### TicketService responsibilities

- Create ticket (default status = `Open`, set timestamps)
- List with optional `search` and `status` query params
- Get by id with comments
- Update fields (exclude status)
- Change status via `StatusTransitionService`

### Validation Strategy

- **Request DTOs** with DataAnnotations (`[Required]`, `[MaxLength]`) and/or FluentValidation
- **FK validation:** verify User exists before assign/create
- **Status transition:** `StatusTransitionService.ValidateTransition`
- Return **ProblemDetails** (RFC 7807) or consistent error envelope:

```json
{
  "title": "Validation failed",
  "status": 400,
  "errors": {
    "Title": ["Title is required."],
    "Status": ["Cannot transition from Open to Resolved."]
  }
}
```

### Error Handling Strategy

| Condition | HTTP Status | Response |
|-----------|-------------|----------|
| Validation failure | 400 | ProblemDetails with field errors |
| Invalid status transition | 400 or 422 | Clear message naming from/to states |
| Not found | 404 | ProblemDetails |
| Unhandled exception | 500 | Generic message (no stack trace in prod) |

Global exception middleware maps domain exceptions to HTTP responses.

---

## Database Design

See `data-model.md` for full detail.

### Key decisions

- **Users:** seeded; unique index on `Email`
- **Tickets:** FK to Users (`CreatedById`, `AssignedToId`); index on `Status`, `UpdatedAt`
- **Comments:** FK to Ticket (cascade delete optional — prefer restrict to preserve history)
- **Enums:** stored as strings in SQL Server for readability in SSMS
- **Timestamps:** `datetime2`, UTC via server

### Connection configuration

- `appsettings.json`: placeholder connection string name
- `appsettings.Development.json`: local dev string (gitignored or uses safe default with Windows auth)
- `appsettings.Development.example.json`: committed example for other developers

---

## Testing Strategy Link

See `test-strategy.md`. Integration tests via `WebApplicationFactory` are the mandatory tier. State machine tests are highest priority.

---

## Security Notes (Core)

- No authentication — all endpoints open (acceptable per assessment).
- No secrets in source — Windows auth only.
- CORS restricted to frontend dev origin in development.

---

## Swagger / OpenAPI

- Enabled in Development environment.
- Documents all endpoints, request/response schemas.
- Used for manual verification during Phase 5.

---

## Deliberate Simplifications

| Area | Decision | Rationale |
|------|----------|-----------|
| Auth | None | Core scope |
| Pagination | None | Stretch |
| Repository pattern | Optional — DbContext in services acceptable for small app | Avoid over-abstraction |
| MediatR/CQRS | Not used | Unnecessary complexity |
| AutoMapper | Optional — manual mapping acceptable for 3 entities | Keep dependencies minimal |
