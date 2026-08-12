# Review Fixes

Applied selectively after Phase 9 AI-assisted review. Re-tested with `dotnet test` after changes.

## Fix 1 — Status filter validation (F1)

**File:** `src/backend/SupportTicket.Api/Controllers/TicketsController.cs`

Reject numeric status values and undefined enum members so only named statuses (`Open`, `InProgress`, …) are accepted.

## Fix 2 — String-only JSON enums (F2)

**File:** `src/backend/SupportTicket.Api/Program.cs`

```csharp
new JsonStringEnumConverter(allowIntegerValues: false)
```

## Fix 3 — Required nullable enums (F3)

**Files:**
- `src/backend/SupportTicket.Application/DTOs/TicketDtos.cs`
- `src/backend/SupportTicket.Application/Validators/TicketValidators.cs`
- `src/backend/SupportTicket.Application/Services/TicketService.cs`

`Priority` / `Status` on requests are nullable with `NotNull()` so omitted fields fail validation instead of silently defaulting.

## Fix 4 — Remove redundant Includes (F4)

**File:** `src/backend/SupportTicket.Application/Services/TicketService.cs`

List query uses projection only; dropped unused `Include` calls.

## Fix 5 — Atomic seed graph save (F5)

**File:** `src/backend/SupportTicket.Infrastructure/Persistence/Seed/DbSeeder.cs`

Ticket + comments saved in a single `SaveChangesAsync` via navigation collection.

## Not Applied

- Search length limit
- Auth / CORS / HTTPS stretch items
- Pagination

## Verification

- `dotnet build` (API)
- `dotnet test` (integration tests)
- Frontend build still succeeds
