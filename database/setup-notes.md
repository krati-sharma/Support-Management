# Database Setup Notes

## Instance

- Server: `localhost\SQLEXPRESS`
- Database: `SupportTicketDb`
- Auth: Windows (`Trusted_Connection=True`)
- Extra: `TrustServerCertificate=True`

## Create / Update

Preferred approach (EF Core migrations):

```powershell
cd src/backend
dotnet ef database update --project SupportTicket.Infrastructure --startup-project SupportTicket.Api
```

Or start the API once — it migrates and seeds automatically in Development.

## Migration Location

- Code: `src/backend/SupportTicket.Infrastructure/Persistence/Migrations/`
- Docs pointer: `database/schema-or-migrations/`

## Seed Location

- Code: `src/backend/SupportTicket.Infrastructure/Persistence/Seed/DbSeeder.cs`
- Docs pointer: `database/seed-data/`

## Verification (2026-08-12)

Queried via `sqlcmd` with TrustServerCertificate:

- Tables: `Users`, `Tickets`, `Comments`, `__EFMigrationsHistory`
- FKs: `FK_Tickets_Users_CreatedById`, `FK_Tickets_Users_AssignedToId`, `FK_Comments_Tickets_TicketId`, `FK_Comments_Users_CreatedById`
- Counts after E2E: Users=4, Tickets≥5 (plus E2E-created), Comments≥3

## SSMS Verification Checklist

- [x] Database `SupportTicketDb` exists
- [x] Tables: `Users`, `Tickets`, `Comments`
- [x] FK constraints present
- [x] Unique index on `Users.Email` (from migration)
- [x] Seed rows present
- [x] Enum values stored as strings
