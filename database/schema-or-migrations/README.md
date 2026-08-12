# Schema / Migrations

EF Core migrations live in:

`src/backend/SupportTicket.Infrastructure/Persistence/Migrations/`

Initial migration: `InitialCreate` (creates Users, Tickets, Comments, indexes, FKs).

Apply with:

```powershell
dotnet ef database update --project SupportTicket.Infrastructure --startup-project SupportTicket.Api
```

Target database: `SupportTicketDb` on `localhost\SQLEXPRESS`.
