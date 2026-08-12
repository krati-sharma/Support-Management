# AI Prompt History — Debugging

## Entry 1 — Tooling and build issues during implementation (2026-08-12)

### Prompt (summary)
Resolve blockers encountered while scaffolding, migrating, testing, and building.

### AI response summary / outcomes
See `debugging-notes.md` for full write-ups:

1. Install `dotnet-ef` 8.0.11
2. Move Vite app to `src/frontend/support-ticket-ui`
3. Add ASP.NET Core framework reference for integration tests
4. Fix TypeScript type-only `FormEvent` imports
5. Stop running API process locking DLLs during rebuild

### Accepted
All five fixes.

### Rejected
N/A.

### Why
Unblocked Core delivery without changing requirements.

### Manually validated
Migrations applied; tests and builds succeeded after each fix.
