# Final AI Usage Summary

## Overview

Cursor AI (Agent mode) was the primary development tool for **Support Ticket Management**. Work followed the assessment’s phased lifecycle: plan → design → setup → database → backend → tests → frontend → E2E → review → documentation. Results were validated with builds, tests, and live API checks — not assumed.

## Prompt Iterations by Phase

| Phase | Prompts (approx.) | Key outcomes |
|-------|-------------------|--------------|
| Planning | 1–2 | `requirements-analysis.md`, `acceptance-criteria.md`, `candidate-info.md` |
| Design | 1 | Architecture, API contract, data model, UI flow, test strategy, implementation plan |
| Implementation | 2+ | .NET solution, EF migrations/seed, APIs, React UI |
| Testing | 1–2 | Integration tests; **15 passed**; `test-results.md` |
| Debugging | Several small | `dotnet-ef` install, path fix, TS imports, file locks |
| Review | 1 | `code-review-notes.md` + selective `review-fixes.md` |
| Documentation | 1 | README, PR description, reflection, this summary |

## Acceptance vs Rejection Rate

- **Accepted:** Core stack, layered architecture, state machine service, SQL Server `SupportTicketDb`, integration tests, review fixes F1–F5
- **Rejected / deferred:** Stretch (auth, user CRUD, Docker, pagination), search max-length hardening, GitHub push (deferred until explicitly requested)

## Lessons Learned

- Phase gates reduce rework and keep prompt history meaningful
- Tooling environment (EF CLI, running processes locking DLLs) matters as much as application code
- Treat AI output as a draft: build, test, and inspect before marking criteria complete
- Keep backend as the source of truth for business rules (status machine)

## References

- Full history: `ai-prompts/`
- Cursor workflow: `tool-specific/cursor-workflow/`
- Test evidence: `test-results.md`
- Review trail: `code-review-notes.md`, `review-fixes.md`
