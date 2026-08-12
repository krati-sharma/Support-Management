# AI Prompt History — Planning

## Prompt 1 — Initial Assessment Kickoff (2026-08-11)

### Prompt (summary)

Full assessment requirements provided: Support Ticket Management System with fixed stack (React/TS/Vite, ASP.NET Core .NET 8, EF Core 8, SQL Server Express `SupportTicketDb` on `localhost\SQLEXPRESS`). Explicit instruction: **Phase 1–2 only** — requirements analysis and planning. Do NOT implement application code until plan is approved.

Key constraints captured:
- .NET 8 only (not .NET 10)
- No auth, no stretch features
- Backend enforces status state machine
- EF Core migrations for database
- Full lifecycle artifacts required
- No GitHub push
- Incremental phased workflow

### AI response summary

1. Inspected repository — empty workspace, no existing project
2. Created `ai-practical-assessment/` folder structure
3. Produced Phase 1 artifacts: `requirements-analysis.md`, `acceptance-criteria.md`, `candidate-info.md`
4. Produced Phase 2 artifacts: `implementation-plan.md`, `design-notes.md`, `data-model.md`, `api-contract.md`, `ui-flow.md`, `test-strategy.md`
5. Created supporting stubs: `README.md`, `tool-workflow.md`, `database/setup-notes.md`
6. Proposed architecture, API surface, UI structure, test strategy
7. Identified assumptions and ambiguities for candidate review
8. Stopped before Phase 3 (no application code)

### What was accepted

- Full phased implementation approach (11 phases)
- Layered backend architecture (Domain, Application, Infrastructure, Api)
- Separate `POST /api/tickets/{id}/status` endpoint for state machine
- Enum values: Priority (Low/Medium/High), Status (Open/InProgress/Resolved/Closed/Cancelled)
- Default ticket status on create: Open
- Search on title + description; status exact filter
- Integration tests with separate `SupportTicketDb_Test` database
- Idempotent seed by User Email
- React Router with 3 pages: List, Create, Detail

### What was modified

_N/A — first planning iteration; awaiting human review._

### What was rejected

_N/A — awaiting human review._

### Why decisions were made

| Decision | Rationale |
|----------|-----------|
| Separate status endpoint | Keeps state machine enforcement explicit; prevents accidental status change via PUT |
| StatusTransitionService in Application layer | Testable, not buried in controller; satisfies assessment judgment requirement |
| String enum storage in SQL | Readable in SSMS; simple EF conversion |
| No auth | Explicitly out of Core scope |
| Test DB separate from dev DB | Avoids test pollution and flaky results |
| validNextStatuses in detail response | Better UX; reduces API round trips |

### What was manually validated

- [x] Repository was empty — confirmed no overwrite of existing work
- [x] Technology stack matches assessment fixed requirements
- [x] Database config points to `SupportTicketDb` not `master`
- [x] All required planning artifact filenames present
- [ ] **Pending:** Candidate review and approval of plan

---

## Next Planning Prompts (anticipated)

- Adjust assumptions based on candidate feedback
- Refine API contract if endpoint structure challenged
- Confirm test database strategy after SQL Server permission check
