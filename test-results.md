# Test Results

## Run Information

| Field | Value |
|-------|-------|
| Date | 2026-08-12 |
| Command | `dotnet test tests/SupportTicket.IntegrationTests/SupportTicket.IntegrationTests.csproj` |
| Project | SupportTicket.IntegrationTests |
| Framework | net8.0 |
| Initial result | **Passed** — Failed: 0, Passed: 15, Skipped: 0 |
| Post review-fix retest | **Passed** — Failed: 0, Passed: 15, Skipped: 0 |

## Coverage Summary

### Valid status transitions (5)

| Transition | Result |
|------------|--------|
| Open → InProgress | Passed |
| InProgress → Resolved | Passed |
| Resolved → Closed | Passed |
| Open → Cancelled | Passed |
| InProgress → Cancelled | Passed |

### Invalid status transitions (8)

| Transition | Result |
|------------|--------|
| Open → Resolved | Passed (400) |
| Open → Closed | Passed (400) |
| Resolved → InProgress | Passed (400) |
| Resolved → Cancelled | Passed (400) |
| Closed → Open | Passed (400) |
| Closed → InProgress | Passed (400) |
| Cancelled → Open | Passed (400) |
| Cancelled → InProgress | Passed (400) |

### Validation / error handling (2)

| Test | Result |
|------|--------|
| Create ticket with missing title → 400 | Passed |
| Get missing ticket → 404 | Passed |

## Notes

- Tests use `WebApplicationFactory` with EF Core **InMemory** provider under `ASPNETCORE_ENVIRONMENT=Testing`.
- Production/runtime persistence remains SQL Server Express `SupportTicketDb` (not used by these integration tests).
- Do not treat InMemory as the application database — it is test isolation only.
