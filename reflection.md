# Reflection

## What I Built

A Core **Support Ticket Management** system: React/Vite frontend, ASP.NET Core Web API on .NET 8, EF Core 8 against SQL Server Express (`SupportTicketDb`). Internal users (seeded) can create, search, filter, update, comment on, and progress tickets through a backend-enforced status state machine. Integration tests prove valid transitions succeed and invalid ones are rejected.

## How I Used AI (across the lifecycle)

Cursor Agent mode was used phase by phase:

1. Requirements analysis and acceptance criteria (no code)
2. Architecture, API contract, data model, UI flow, test strategy
3. Scaffolding projects and implementing backend/frontend incrementally
4. Writing and running integration tests; recording real results
5. Debugging build/tooling issues (`dotnet-ef`, file locks, TypeScript imports)
6. AI-assisted code review with selective fixes
7. Completing README and lifecycle artifacts

Prompt history is recorded under `ai-prompts/` (not a single giant generation prompt).

## What AI Helped With Most

- Turning the assessment brief into concrete planning artifacts and a phased plan
- Scaffolding the layered .NET solution and React app structure quickly
- Implementing the state machine + API surface consistently with the contract
- Drafting integration tests and documentation templates

## What AI Got Wrong

- Initially created the Vite app in the wrong directory (repo root vs `src/frontend/`)
- Needed correction for TypeScript `verbatimModuleSyntax` type-only imports
- Integration test host needed an explicit ASP.NET Core framework reference
- Occasional parallel build file-lock issues when the API was still running

These were caught by build/test failures and fixed before claiming success.

## How I Validated AI Output

- `dotnet build` / `dotnet test` (15/15 passed)
- `dotnet ef database update` against `SupportTicketDb`
- Live API calls for create/update/comment/search/filter/transitions
- `npm run build` for the frontend
- Selective code review; Stretch suggestions rejected

## What I Would Improve Next

- Add a short UI smoke checklist script or Playwright stretch tests
- Harden search input length
- After Core sign-off, consider Stretch auth + pagination
- Keep commits smaller per phase when publishing to GitHub

## Reusable Workflow (prompts, rules, specs, templates)

- Phase-gated prompts (“planning only”, then “implement Phase N”)
- Fixed stack constraints in every implementation prompt (net8.0, SQL Express, no Stretch)
- Keep `api-contract.md` / `data-model.md` as the source of truth before coding
- Always run tests and paste real results into `test-results.md`
- Cursor workflow notes in `tool-specific/cursor-workflow/`
