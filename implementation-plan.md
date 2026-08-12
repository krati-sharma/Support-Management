# Implementation Plan

## Overview

Build a Support Ticket Management System incrementally across 11 phases, validating each before proceeding. Phases 1–2 (requirements and design) are complete pending your review. Implementation begins only after explicit approval.

**Goal:** Deliver Core functionality with clean architecture, enforced status state machine, SQL Server persistence, React UI, integration tests, and full lifecycle documentation.

---

## Proposed Solution Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    React + TypeScript (Vite)                 │
│  Pages: List | Create | Detail (update, status, comments)   │
│  State: local component state + fetch API                   │
└──────────────────────────┬──────────────────────────────────┘
                           │ HTTP/JSON (REST)
                           ▼
┌─────────────────────────────────────────────────────────────┐
│              SupportTicket.Api (.NET 8 Web API)              │
│  Controllers (thin) → Application Services → Domain rules  │
│  Swagger, validation filters, global exception handling      │
└──────────────────────────┬──────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────────┐
│         SupportTicket.Infrastructure (EF Core 8)             │
│  AppDbContext, Repositories (if needed), Migrations, Seed    │
└──────────────────────────┬──────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────────┐
│     SQL Server Express — SupportTicketDb                     │
│     localhost\SQLEXPRESS                                     │
└─────────────────────────────────────────────────────────────┘
```

### Backend Projects

| Project | Responsibility |
|---------|----------------|
| `SupportTicket.Domain` | Entities, enums (`TicketStatus`, `Priority`), domain exceptions |
| `SupportTicket.Application` | DTOs, services (`TicketService`, `StatusTransitionService`), validation interfaces |
| `SupportTicket.Infrastructure` | EF Core `DbContext`, configurations, migrations, seeding |
| `SupportTicket.Api` | Controllers, DI wiring, Swagger, CORS, middleware |

### Frontend Project

| Path | Responsibility |
|------|----------------|
| `src/frontend/support-ticket-ui/` | Vite React app, pages, components, API client, types |

### Test Project

| Path | Responsibility |
|------|----------------|
| `tests/SupportTicket.IntegrationTests/` | WebApplicationFactory, state machine tests, validation tests |

---

## Task Breakdown

### Phase 1 — Requirements Analysis ✅ (this session)

- [x] Analyze assessment requirements
- [x] Create `requirements-analysis.md`
- [x] Create `acceptance-criteria.md`
- [x] Create `candidate-info.md` (candidate fields TBD)

### Phase 2 — Architecture and Design ✅ (this session, pending review)

- [x] Create `implementation-plan.md`
- [x] Create `design-notes.md`
- [x] Create `data-model.md`
- [x] Create `api-contract.md`
- [x] Create `ui-flow.md`
- [x] Create `test-strategy.md`

### Phase 3 — Project Setup (after approval)

- [x] Initialize Git repo and `.gitignore`
- [x] Create backend solution with 4 projects targeting `net8.0`
- [x] Create React + Vite + TypeScript frontend via `npm create vite`
- [x] Create integration test project (xUnit)
- [x] Configure CORS, Swagger, connection string placeholder
- [x] Verify `dotnet build` and `npm run build` succeed

### Phase 4 — Database

- [x] Define domain entities and enums
- [x] Configure EF Core relationships and indexes
- [x] Create initial migration
- [x] Implement idempotent seed data (users, sample tickets, comments)
- [x] Apply migration to `SupportTicketDb`
- [ ] Verify schema and seed in SSMS (candidate)

### Phase 5 — Backend

- [x] Implement `StatusTransitionService` with unit-testable logic
- [x] Implement `TicketService` and comment operations
- [x] Implement controllers per API contract
- [x] Add FluentValidation + custom validation
- [x] Global exception → ProblemDetails mapping
- [x] Users read-only endpoint for dropdown population
- [x] Manual API verification via HTTP (`/api/users`, `/api/tickets`)

### Phase 6 — Integration Tests

- [x] WebApplicationFactory setup
- [x] State machine valid/invalid transition tests
- [x] Validation error tests
- [x] Run tests; record results in `test-results.md`

### Phase 7 — Frontend

- [x] API client module with typed responses/errors
- [x] Ticket list page (search, status filter, loading/empty)
- [x] Create ticket page/form
- [x] Ticket detail page (view, edit, status change, comments)
- [x] Form validation and API error display
- [x] Configure dev proxy to API

### Phase 8 — End-to-End Verification

- [x] Full stack run: React → API → SQL Server (API E2E + UI implemented; UI walkthrough documented)
- [x] Restart persistence check (ticket remained Closed with comment in SQL Server)
- [x] Walk through acceptance criteria
- [x] Update checklist with evidence (`acceptance-criteria.md`, `FINAL-VERIFICATION.md`)

### Phase 9 — Code Review

- [x] AI-assisted review → `code-review-notes.md`
- [x] Selective fixes → `review-fixes.md`

### Phase 10 — Documentation

- [x] Complete `README.md`
- [x] Finalize prompt history in `ai-prompts/`
- [x] Complete `pr-description.md`, `reflection.md`, `final-ai-usage-summary.md`
- [x] `debugging-notes.md` as issues arise

### Phase 11 — Final Verification

- [x] Build frontend + backend
- [x] Run all tests
- [x] Verify no secrets committed
- [x] Final checklist with evidence
- [ ] Final checklist with evidence

---

## Milestones

| Milestone | Deliverable | Gate |
|-----------|-------------|------|
| M1 | Planning artifacts approved | **Your review (current gate)** |
| M2 | Projects build successfully | Phase 3 complete |
| M3 | Database migrated and seeded | Phase 4 + SSMS check |
| M4 | API functional via Swagger | Phase 5 |
| M5 | Integration tests green | Phase 6 + `test-results.md` |
| M6 | UI connected and functional | Phase 7 |
| M7 | E2E verified | Phase 8 |
| M8 | Submission-ready repo | Phase 11 |

---

## AI Usage Plan

| Phase | AI role | Human validation |
|-------|---------|------------------|
| Planning | Analyze requirements, draft artifacts | Review assumptions, approve plan |
| Setup | Scaffold projects, `.gitignore` | Verify builds, .NET 8 target |
| Database | Entity/config/migration/seed code | SSMS inspection, migration re-run |
| Backend | Services, controllers, validation | Swagger manual tests |
| Tests | Generate test cases from state machine | Run `dotnet test`, verify results |
| Frontend | Pages, forms, error handling | Browser manual testing |
| Review | AI code review suggestions | Selectively accept/reject |
| Docs | README drafts, prompt history | Accuracy check, no fabrication |

---

## Risks

| Risk | Impact | Mitigation |
|------|--------|------------|
| SQL Server Express not running | Blocks DB/migrations/tests | Document prerequisite; verify connection early in Phase 4 |
| .NET 10 installed but must use .NET 8 | Wrong TFM | Explicit `net8.0` in all `.csproj`; global.json optional |
| State machine logic only in controller | Untestable, fragile | Dedicated `StatusTransitionService` in Application layer |
| Seed duplicates on re-run | Data integrity issues | Idempotent seed by Email / deterministic keys |
| Integration tests pollute dev DB | Flaky tests | Use separate test DB or transactional cleanup |
| Frontend/backend CORS issues | Blocked API calls | Configure CORS in API; Vite proxy in dev |
| Over-engineering | Scope creep | Stick to Core; no auth/pagination/Docker |

---

## Mitigation

- Validate SQL Server connectivity in Phase 3 before heavy DB work.
- Enforce state machine in one service with explicit transition map — covered by integration tests.
- Keep controllers thin; use DTOs at API boundary.
- Record actual test output in `test-results.md` — never fabricate.
- Incremental phases with build/test gates between each.

---

## Estimated Repository Layout (post-implementation)

```
ai-practical-assessment/
├── src/
│   ├── frontend/support-ticket-ui/
│   └── backend/
│       ├── SupportTicket.Api/
│       ├── SupportTicket.Application/
│       ├── SupportTicket.Domain/
│       └── SupportTicket.Infrastructure/
├── tests/
│   └── SupportTicket.IntegrationTests/
└── database/
    ├── schema-or-migrations/    ← EF migration snapshots or export notes
    ├── seed-data/               ← seed script reference / JSON samples
    └── setup-notes.md
```

---

## Approval Required

**Implementation (Phase 3+) will not begin until you explicitly approve this plan.**

Please review:

1. Assumptions (especially default status `Open`, priority enum, search scope)
2. API endpoint design in `api-contract.md`
3. Data model in `data-model.md`
4. UI structure in `ui-flow.md`
5. Test database strategy in `test-strategy.md`

Reply with approval or requested changes.
