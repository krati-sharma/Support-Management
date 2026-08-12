# Requirement Analysis

## Selected Project Option

**Option 1: Backend-Heavy — Support Ticket Management System (Core tier only)**

Stretch features (authentication, user CRUD, pagination, Docker, CI, etc.) are explicitly out of scope unless requested later.

---

## My Understanding (in your own words)

This assessment evaluates **AI-assisted full-stack engineering workflow**, not just the ability to produce working code. The deliverable is a small but complete Support Ticket Management System backed by SQL Server Express, with a React frontend and ASP.NET Core API, plus a rich set of lifecycle artifacts documenting planning, design, implementation, testing, review, and reflection.

The **signature business rule** is the ticket status state machine: only five valid transitions exist, and the **backend must enforce** them. Invalid transitions must return meaningful HTTP errors that the frontend surfaces to the user.

Users exist as seeded data only — there is no login, no user CRUD, and no role-management UI. The UI will allow selecting seeded users as `CreatedBy` and `AssignedTo` when creating/updating tickets and comments.

Data must persist across application restarts via EF Core migrations against `SupportTicketDb` on `localhost\SQLEXPRESS`.

---

## Functional Requirements

### Entities

| Entity | Fields | Notes |
|--------|--------|-------|
| **User** | Id, Name, Email, Role | Seeded only; no management UI |
| **Ticket** | Id, Title, Description, Priority, Status, AssignedTo, CreatedBy, CreatedAt, UpdatedAt | FK to User for assignee and creator |
| **Comment** | Id, TicketId, Message, CreatedBy, CreatedAt | FK to Ticket and User |

### Ticket Lifecycle (State Machine)

| From | To |
|------|-----|
| Open | In Progress |
| In Progress | Resolved |
| Resolved | Closed |
| Open | Cancelled |
| In Progress | Cancelled |

All other transitions are invalid. Closed and Cancelled are terminal states.

### Core Features

1. **Create ticket** — title, description, priority, assignee, creator (from seeded users).
2. **List tickets** — from database with loading/empty states.
3. **View ticket detail** — includes comments.
4. **Update ticket** — title, description, priority, assignee (not status via general update).
5. **Change status** — dedicated action enforcing state machine.
6. **Add comments** — message + creator.
7. **Keyword search** — search across ticket fields (title, description at minimum).
8. **Filter by status** — filter ticket list.
9. **Persist data** — SQL Server via EF Core migrations.
10. **Backend validation** — required fields rejected with meaningful errors.
11. **Frontend validation/errors** — UX improvements; backend is authoritative.
12. **API error handling** — clear messages in React UI.

### API

- RESTful HTTP JSON API
- Documented in `api-contract.md`
- Swagger/OpenAPI available at runtime

### Testing

- ASP.NET Core integration tests (mandatory)
- Primary focus: state machine valid/invalid transitions
- Results recorded in `test-results.md` after actual execution

---

## Non-Functional Requirements

| Category | Requirement |
|----------|-------------|
| **Technology** | Fixed stack: React/TS/Vite, ASP.NET Core .NET 8, EF Core 8, SQL Server Express |
| **Database** | `SupportTicketDb` on `localhost\SQLEXPRESS`; Windows auth; no SQLite/LocalDB/Docker DB |
| **Security** | No secrets in Git; connection string via configuration; `Trusted_Connection=True` |
| **Architecture** | Clean separation: Domain, Application, Infrastructure, API layers |
| **Maintainability** | Thin controllers; state machine in testable service; DTOs at API boundary |
| **Simplicity** | No auth, microservices, Redis, message queues, or unnecessary libraries |
| **Documentation** | Full lifecycle artifacts; README with setup instructions |
| **Git** | Local repo with `.gitignore`; no push to GitHub unless explicitly requested |
| **AI workflow** | Iterative prompt history; human validation of AI output |

---

## Assumptions

1. **SQL Server Express** is installed and reachable at `localhost\SQLEXPRESS` with Windows Authentication.
2. **Initial ticket status** on create is `Open` (not specified in requirements; standard default).
3. **Priority** is an enum with values such as `Low`, `Medium`, `High` (exact set to be defined in design).
4. **Role** on User is informational only in Core (no authorization based on role).
5. **CreatedBy / AssignedTo** reference seeded User IDs; UI presents a dropdown of seeded users.
6. **Keyword search** applies to `Title` and `Description` (minimum viable; can extend to comment text in Stretch).
7. **Status filter** is exact match on ticket status enum.
8. **Ticket list** returns all matching tickets without pagination (Stretch feature).
9. **Comments** are append-only in Core (no edit/delete).
10. **Timestamps** (`CreatedAt`, `UpdatedAt`) are set by the server (UTC).
11. **General ticket update (PUT/PATCH)** does not allow direct status changes — status uses a dedicated endpoint.
12. **Seed data** is idempotent (check-before-insert or upsert by natural key such as Email for users).
13. **Frontend dev server** proxies API calls or uses configurable base URL (e.g., `VITE_API_URL`).
14. **Integration tests** use the same SQL Server instance with a test database or transactional rollback strategy (to be decided in test strategy — see ambiguities).

---

## Clarifications (questions for a product owner)

| # | Question | Proposed default if unanswered |
|---|----------|-------------------------------|
| 1 | Which **Priority** values are allowed? | `Low`, `Medium`, `High` |
| 2 | Which **Role** values for seeded users? | `Agent`, `Supervisor`, `Admin` (display only) |
| 3 | Should keyword search include **comment text**? | No in Core; title + description only |
| 4 | Can **AssignedTo** be null/unassigned? | No — required on create and update |
| 5 | Should list endpoint support **sorting** (e.g., by UpdatedAt desc)? | Default sort by `UpdatedAt` descending; no UI sort control in Core |
| 6 | Integration test database strategy? | Separate `SupportTicketDb_Test` or use WebApplicationFactory with same DB + cleanup |
| 7 | Minimum **seed data** volume? | 3–5 users, 5–10 tickets, 2–5 comments per some tickets |

---

## Edge Cases

| Scenario | Expected behavior |
|----------|-------------------|
| Invalid status transition | HTTP 400/422 with clear error message; UI displays error |
| Missing required fields on create/update | HTTP 400 with validation details |
| Ticket ID not found | HTTP 404 |
| User ID not found (assignee/creator) | HTTP 400/404 with validation error |
| Comment on non-existent ticket | HTTP 404 |
| Empty search keyword | Return all tickets (or unfiltered list) |
| Status filter with no matches | Empty list with empty state in UI |
| Closed/Cancelled ticket update | Allow field updates? **Assumption:** yes for title/description/priority/assignee; status change still blocked |
| Duplicate seed on re-migration | Idempotent seed — no duplicate users/tickets |
| SQL Server unavailable | API returns 500; frontend shows connection error |
| Very long title/description | Apply reasonable max length validation (e.g., 200/4000 chars) |

---

## Technology Stack Confirmation

| Layer | Technology | Version constraint |
|-------|------------|-------------------|
| Frontend | React, TypeScript, Vite | npm package manager |
| Backend | ASP.NET Core Web API | **.NET 8** (`net8.0`) — NOT .NET 10 |
| ORM | Entity Framework Core | **8.x** |
| Database | SQL Server Express | `localhost\SQLEXPRESS` |
| Database name | `SupportTicketDb` | Not `master` or system DBs |
| Connection string (runtime) | `Server=localhost\SQLEXPRESS;Database=SupportTicketDb;Trusted_Connection=True;TrustServerCertificate=True;` | Via configuration |
| Testing | xUnit + ASP.NET Core integration tests | Mandatory state machine tests |

---

## Database Configuration Confirmation

- **Instance:** `localhost\SQLEXPRESS`
- **Database:** `SupportTicketDb` (created via EF Core migrations)
- **Auth:** Windows Authentication (`Trusted_Connection=True`)
- **Management:** SSMS for verification; no manual DB creation unless troubleshooting
- **Persistence:** Data survives app restart
- **Migrations:** EF Core handles schema, FKs, indexes, and seed data

---

## Out of Scope (Core)

- Authentication / authorization
- User CRUD / role management UI
- Pagination, advanced sorting, priority/assignee filters
- Docker, CI/CD, Redis, message queues
- Microservices
- Stretch features unless explicitly requested
