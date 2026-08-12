# AI Prompt History — Implementation

## Entry 1 — Start Core implementation after plan approval (2026-08-12)

### Prompt (summary)
User approved the planning artifacts and asked to start development. Also requested renaming the project to **Support Ticket Management**.

### AI response summary
Implemented Phases 3–7 incrementally:
- Scaffolded .NET 8 layered backend + Vite React frontend + xUnit integration tests
- EF Core SQL Server `SupportTicketDb` migration + idempotent seed
- Ticket/User APIs, FluentValidation, status state machine service
- React list/create/detail UI with search, filter, comments, status transitions
- Recorded real test results (15 passed)

### Accepted
- Full Core implementation as designed in Phase 2 artifacts
- Display name **Support Ticket Management** (repo folder remains `ai-practical-assessment` per assessment structure)

### Modified
- Integration tests use InMemory EF only under `Testing` environment (SQL Server remains runtime DB)
- Frontend created under `src/frontend/support-ticket-ui`

### Rejected
- Stretch features (auth, user CRUD, Docker, pagination)
- Pushing to GitHub / creating remotes

### Why
Matches approved plan and assessment Core scope; keeps GitHub operations local-only.

### Manually validated
- `dotnet build` succeeded for API
- `dotnet ef database update` created `SupportTicketDb`
- `dotnet test` → 15 passed / 0 failed
- `npm run build` succeeded for frontend
