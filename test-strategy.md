# Test Strategy

## Test Scope

### In Scope (Core — Mandatory)

| Tier | Focus |
|------|-------|
| **Integration tests** | API endpoints via `WebApplicationFactory` |
| **State machine** | All valid and invalid transitions (highest priority) |
| **Validation** | Required fields, invalid FKs, bad enum values |
| **HTTP semantics** | Correct status codes (200, 201, 400, 404) |

### Out of Scope (Core)

| Tier | Reason |
|------|--------|
| Unit tests | Optional; integration tests satisfy mandatory tier |
| Frontend component tests | Not required in Core |
| E2E browser tests (Playwright/Cypress) | Manual E2E verification sufficient |
| Performance/load tests | Not required |
| Security/auth tests | No auth in Core |

Stretch may add unit tests and frontend tests if requested later.

---

## Test Project Structure

```
tests/
└── SupportTicket.IntegrationTests/
    ├── SupportTicket.IntegrationTests.csproj
    ├── CustomWebApplicationFactory.cs
    ├── TicketStatusTransitionTests.cs    ← primary
    ├── TicketValidationTests.cs
    └── TicketCrudTests.cs                  ← basic CRUD smoke tests
```

---

## Integration Test Setup

### WebApplicationFactory

- Bootstraps full API with test configuration
- Overrides connection string to test database

### Database Strategy (proposed)

**Option A — Separate test database (recommended):**

```
Server=localhost\SQLEXPRESS;Database=SupportTicketDb_Test;Trusted_Connection=True;TrustServerCertificate=True;
```

- Apply migrations before test run
- Truncate or recreate between test classes if needed
- Does not pollute dev `SupportTicketDb`

**Option B — Shared database with cleanup:**

- Use dev DB but delete test-created records after each test
- Higher risk of interference; not preferred

**Decision:** Use **Option A** (`SupportTicketDb_Test`) unless SQL Server permissions prevent creating a second database.

### Seed data in tests

- Tests may rely on seeded users from migration seed
- Tests create tickets via API for transition testing (deterministic titles)
- Each test should arrange its own ticket in known starting status

---

## State Machine Tests (Mandatory)

### Valid transitions — must return 200 and updated status

| Test name | Arrange | Act | Assert |
|-----------|---------|-----|--------|
| `Open_To_InProgress_Succeeds` | Ticket status Open | POST status InProgress | 200, status InProgress |
| `InProgress_To_Resolved_Succeeds` | Ticket InProgress | POST status Resolved | 200, status Resolved |
| `Resolved_To_Closed_Succeeds` | Ticket Resolved | POST status Closed | 200, status Closed |
| `Open_To_Cancelled_Succeeds` | Ticket Open | POST status Cancelled | 200, status Cancelled |
| `InProgress_To_Cancelled_Succeeds` | Ticket InProgress | POST status Cancelled | 200, status Cancelled |

### Invalid transitions — must return 400/422 with error message

| Test name | Arrange | Act | Assert |
|-----------|---------|-----|--------|
| `Open_To_Resolved_Fails` | Ticket Open | POST status Resolved | 400, error mentions invalid transition |
| `Open_To_Closed_Fails` | Ticket Open | POST status Closed | 400 |
| `Resolved_To_InProgress_Fails` | Ticket Resolved | POST status InProgress | 400 |
| `Resolved_To_Cancelled_Fails` | Ticket Resolved | POST status Cancelled | 400 |
| `Closed_To_Open_Fails` | Ticket Closed | POST status Open | 400 |
| `Closed_To_InProgress_Fails` | Ticket Closed | POST status InProgress | 400 |
| `Cancelled_To_Open_Fails` | Ticket Cancelled | POST status Open | 400 |
| `Cancelled_To_InProgress_Fails` | Ticket Cancelled | POST status InProgress | 400 |

---

## Validation Tests (Recommended)

| Test | Expected |
|------|----------|
| Create ticket without title | 400 |
| Create ticket without description | 400 |
| Create ticket with invalid priority | 400 |
| Create ticket with non-existent user ID | 400 |
| Add comment without message | 400 |
| Get non-existent ticket | 404 |
| Status change on non-existent ticket | 404 |

---

## CRUD Smoke Tests (Recommended)

| Test | Expected |
|------|----------|
| Create ticket | 201, Location header, status Open |
| List tickets | 200, contains created ticket |
| Get ticket by id | 200, correct fields |
| Update ticket title | 200, title changed, status unchanged |
| Add comment | 201, comment appears on detail |

---

## Search and Filter Tests (Optional but valuable)

| Test | Expected |
|------|----------|
| Search by keyword in title | Returns matching tickets |
| Filter by status Open | Returns only Open tickets |
| Search + filter combined | Both constraints applied |

---

## Tests Not Covered (and why)

| Area | Reason |
|------|--------|
| Frontend UI tests | Manual verification; not mandatory tier |
| Concurrent status updates | Edge case; Stretch |
| SQL injection | Parameterized EF queries; low risk for assessment |
| Auth/authorization | Not in Core |
| Pagination | Not in Core |

---

## Running Tests

```powershell
cd ai-practical-assessment
dotnet test tests/SupportTicket.IntegrationTests/SupportTicket.IntegrationTests.csproj --verbosity normal
```

Results recorded in `test-results.md` after actual execution (Phase 6).

---

## Success Criteria

- All mandatory state machine tests pass
- Validation tests pass where implemented
- Zero test failures before marking testing acceptance criteria complete
- Test output copied to `test-results.md` (not fabricated)

---

## Test Data Isolation

Each transition test pattern:

```csharp
// Arrange
var createResponse = await client.PostAsJsonAsync("/api/tickets", new { ... });
var ticket = await createResponse.Content.ReadFromJsonAsync<TicketDetailDto>();

// Act
var statusResponse = await client.PostAsJsonAsync(
    $"/api/tickets/{ticket.Id}/status",
    new { status = "InProgress" });

// Assert
statusResponse.StatusCode.Should().Be(HttpStatusCode.OK);
```

For invalid transition tests, first transition ticket to required starting state via valid steps.

---

## Tools

- **xUnit** — test framework
- **FluentAssertions** — readable assertions (optional, common in .NET tests)
- **Microsoft.AspNetCore.Mvc.Testing** — WebApplicationFactory
