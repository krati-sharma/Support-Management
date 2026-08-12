# Git & Prompt History Alignment

This repository follows the GenAI assessment Git guidelines.

## Branch model

| Branch | Purpose |
|--------|---------|
| `main` | Stable Core delivery (promoted from `dev`) |
| `dev` | Integration branch for feature/fix work |
| `feature/*` | Incremental implementation slices |
| `fix/*` | Review/hardening follow-ups |

## Commit ↔ prompt alignment

| Commit / branch theme | Prompt history |
|-----------------------|----------------|
| Planning/design docs on `main` | `ai-prompts/planning.md`, `ai-prompts/design.md` |
| `feature/project-setup` | `ai-prompts/implementation.md` (scaffold) |
| `feature/database` | `ai-prompts/implementation.md` (EF/seed) |
| `feature/backend-api` | `ai-prompts/implementation.md` (API/state machine) |
| `feature/integration-tests` | `ai-prompts/testing.md` |
| `feature/frontend-ui` | `ai-prompts/implementation.md` (UI) |
| `fix/code-review-hardening` | `ai-prompts/code-review.md`, `ai-prompts/debugging.md` |
| `feature/docs-finalization` | `ai-prompts/documentation.md` |

## Notes

- Local folder name remains `ai-practical-assessment` (required assessment structure).
- GitHub repository name: `Support-Ticket-Management` (product name; no code namespace changes required).
- Commits intentionally reflect iterative delivery rather than a single bulk upload.
