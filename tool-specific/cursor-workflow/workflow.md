# Cursor Workflow — Support Ticket Assessment

## Tool Selection

**Cursor AI — Agent mode** is the primary development tool for this assessment.

Only `tool-specific/cursor-workflow/` is populated. Other tool folders remain empty.

## Agent Mode Usage

### Phase gating

The assessment explicitly requires incremental delivery. In Cursor Agent mode:

1. **One phase at a time** — do not generate entire application in single prompt
2. **Inspect before modify** — agent checks repository state before changes
3. **Build/test after changes** — verify each phase before proceeding
4. **Wait for approval** — planning gate before Phase 3

### Effective prompts observed

| Good pattern | Example |
|--------------|---------|
| Scope constraint | "Phase 1 only — do not code yet" |
| Stack constraint | "Target net8.0, EF Core 8, SQL Server Express" |
| Validation request | "Run dotnet test and record results in test-results.md" |
| Incremental | "Implement backend ticket API only; no frontend yet" |

| Weak pattern | Why |
|--------------|-----|
| "Build the entire app" | Violates phased workflow; hard to validate |
| No stack constraints | Risk of wrong TFM or database |
| "Mark tests as passed" | Fabrication — must run tests |

## Repository Conventions

- Workspace root: `ai-practical-assessment/`
- Application code under `src/`
- Tests under `tests/`
- Lifecycle docs at repository root
- Prompt history under `ai-prompts/`

## Cursor-Specific Validation Steps

After each implementation phase:

1. **Terminal:** `dotnet build`, `dotnet test`, `npm run build`
2. **SSMS:** Verify database schema and seed data
3. **Browser:** Manual UI walkthrough
4. **Swagger:** API endpoint verification
5. **Artifacts:** Update relevant markdown files with real results

## Git in Cursor

- Initialize Git in Phase 3
- Use `.gitignore` for `node_modules/`, `bin/`, `obj/`, secrets
- Local commits only — no push unless explicitly requested

## Human-in-the-Loop Checkpoints

| Gate | Action |
|------|--------|
| After planning | Review and approve plan |
| After database migration | SQL / SSMS verification |
| After backend | Swagger / HTTP testing |
| After integration tests | Review test-results.md |
| After frontend | Browser E2E |
| Before submission | Final checklist — **complete** (`FINAL-VERIFICATION.md`) |

## Recording AI Usage

For each meaningful Cursor session, append to the appropriate file in `ai-prompts/`:
- Prompt text or summary
- AI response summary
- Accepted / modified / rejected
- Manual validation performed

See `ai-prompts/planning.md` for first entry from this session.
