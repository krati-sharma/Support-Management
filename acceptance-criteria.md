# Acceptance Criteria

Checklist for Core delivery. Status updated during implementation and final verification.

## Core Features

- [x] A user can **create a ticket** via the UI (title, description, priority, assignee, creator). *(API E2E + UI implemented)*
- [x] A user can **view all tickets** loaded from the database (not hardcoded). *(API list + seed)*
- [x] A user can open a **ticket detail view** showing ticket fields and comments.
- [x] A user can **update ticket fields** (title, description, priority, assignee/reassign).
- [x] A user can **add comments** to a ticket.
- [x] **Status changes** succeed only through valid transitions; invalid transitions are rejected by the API.
- [x] **Keyword search** filters tickets by search term (title/description).
- [x] **Status filter** filters tickets by status.
- [x] **Data persists** after application operations/restart (ticket #6 remained Closed with comment in SQL Server).
- [x] **Backend validation** prevents invalid records (missing required fields, invalid FKs).
- [x] **No secrets** committed to the repository (Windows auth connection strings only).
- [x] **State-machine integration tests** pass (recorded in `test-results.md` — 15/15).

## Status State Machine

### Valid transitions (must succeed)

- [x] Open → In Progress *(tests + E2E)*
- [x] In Progress → Resolved
- [x] Resolved → Closed
- [x] Open → Cancelled
- [x] In Progress → Cancelled *(integration tests)*

### Invalid transitions (must be rejected by backend)

- [x] Open → Resolved
- [x] Open → Closed
- [x] Resolved → In Progress
- [x] Resolved → Cancelled
- [x] Closed → any other state
- [x] Cancelled → any other state

## Validation

- [x] Ticket title required on create/update.
- [x] Ticket description required on create/update.
- [x] Priority required and must be valid enum value.
- [x] CreatedBy required and must reference existing user.
- [x] AssignedTo required and must reference existing user.
- [x] Comment message required.
- [x] Comment CreatedBy required and must reference existing user.
- [x] API returns structured validation errors (HTTP 400).
- [x] Frontend displays validation messages from API.

## Error Handling

- [x] HTTP 404 for ticket/user not found where applicable.
- [x] HTTP 400 for invalid status transition with meaningful message.
- [x] Frontend shows API error messages (not silent failures).
- [x] Loading states shown during API calls.
- [x] Empty states shown when no tickets match search/filter.

## Testing

- [x] Integration test project exists under `tests/`.
- [x] All mandatory state machine tests implemented.
- [x] Tests actually executed; results in `test-results.md`.
- [x] Build succeeds for backend and frontend.

## Database

- [x] Database name is `SupportTicketDb` on `localhost\SQLEXPRESS`.
- [x] EF Core migrations create schema (Users, Tickets, Comments).
- [x] Foreign keys and relationships correct (verified via SQL catalog query).
- [x] Seed data present and idempotent.
- [x] Connection string in configuration (not hardcoded secrets).

## Documentation

- [x] `README.md` with setup, run, migration, and test instructions.
- [x] `api-contract.md` documents all endpoints.
- [x] `data-model.md` documents entities and relationships.
- [x] All required lifecycle artifacts present in repository structure.
- [x] `ai-prompts/` contains accurate prompt history (not fabricated).
- [x] `tool-specific/cursor-workflow/` populated for Cursor workflow.

## UI

- [x] Ticket list page with search and status filter.
- [x] Ticket creation form.
- [x] Ticket detail view with update form.
- [x] Status transition control (only valid next states offered where practical).
- [x] Comment creation on detail view.
- [x] Assignee selection from seeded users.
- [x] Creator selection from seeded users (create/comment flows).

## Candidate Fields

- [x] `candidate-info.md` — name/role filled in.
- [x] `reflection.md` — completed from actual project experience.
- [x] Demo notes in `pr-description.md`.
