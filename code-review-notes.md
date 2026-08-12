# Code Review Notes

## AI-Assisted Review Summary

Reviewed backend Core implementation after Phase 7–8 delivery (controllers, services, validators, middleware, seeder, Program.cs). Focused on correctness against the API contract and Core acceptance criteria — not Stretch features.

### Findings

| ID | Severity | Finding | Decision |
|----|----------|---------|----------|
| F1 | Medium | Status query filter accepted undefined/numeric enum values via `Enum.TryParse` | **Accepted** |
| F2 | Medium | `JsonStringEnumConverter` allowed integer enum JSON payloads | **Accepted** |
| F3 | Medium | Non-nullable enum DTO fields defaulted when omitted, bypassing “required” intent | **Accepted** |
| F4 | Medium | Redundant `Include` before `Select` projection in ticket list query | **Accepted** |
| F5 | Medium | Seeder saved ticket then comments separately (partial seed risk) | **Accepted** |
| F6 | Medium | Unbounded search string length | **Rejected** (not Core AC) |
| Auth / HTTPS / pagination | — | Suggested hardening | **Rejected** (out of Core scope) |

## My Review Observations

- State machine is correctly centralized in `StatusTransitionService` and enforced by the API.
- Controllers stay thin; validation and transition rules live in Application layer.
- SQL Server connection uses Windows auth only — no secrets in repo.
- Integration tests cover the mandatory state-machine matrix.

## Changes Made After Review

See `review-fixes.md`.

## Suggestions Rejected (and why)

- Search max-length hardening — nice-to-have, not required for Core.
- Auth / role checks — Stretch.
- Pagination / sorting extras — Stretch.
- Comment `Location` header polish — cosmetic REST nit.
