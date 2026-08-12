# Candidate Information

| Field | Value |
|-------|-------|
| **Name** | Krati Sharma |
| **Role** | Full-Stack Developer |
| **Primary Technology Stack** | React, TypeScript, ASP.NET Core, .NET 8, SQL Server Express |
| **Primary AI Tool Used** | Cursor AI (Agent mode) |
| **Project Option Selected** | Option 1 — Support Ticket Management (Core only) |
| **Assessment Start Date** | 2026-08-11 |
| **Submission Date** | 2026-08-12 |

## Project Summary

A full-stack **Support Ticket Management** application for internal users to create, search, filter, update, comment on, and progress tickets through an enforced lifecycle state machine. The application uses React + TypeScript (Vite) on the frontend, ASP.NET Core Web API (.NET 8) on the backend, and Microsoft SQL Server Express (`SupportTicketDb`) for persistence. Users are seeded only; no authentication or user-management UI in Core scope.

## Tools Used

- **IDE / AI:** Cursor (Agent mode)
- **Frontend:** React, TypeScript, Vite, npm
- **Backend:** ASP.NET Core Web API, .NET 8, C#, EF Core 8
- **Database:** SQL Server Express (`localhost\SQLEXPRESS`, database `SupportTicketDb`)
- **Testing:** xUnit, ASP.NET Core integration tests
- **API docs:** Swagger/OpenAPI (built-in)
- **DB inspection:** SQL Server Management Studio (SSMS)
- **Version control:** Git (local only until explicitly published)

## Setup Summary

1. Ensure SQL Server Express is running at `localhost\SQLEXPRESS`.
2. Connection string targets `SupportTicketDb` (see `appsettings.Development.json`).
3. Start API (`dotnet run` in `src/backend/SupportTicket.Api`) — applies migrations + seed.
4. Start UI (`npm run dev` in `src/frontend/support-ticket-ui`).
5. Run tests: `dotnet test tests/SupportTicket.IntegrationTests`.
6. Full steps: see `README.md`.
