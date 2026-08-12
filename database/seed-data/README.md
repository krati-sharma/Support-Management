# Seed Data

Runtime seeder:

`src/backend/SupportTicket.Infrastructure/Persistence/Seed/DbSeeder.cs`

## Users

| Name | Email | Role |
|------|-------|------|
| Alice Agent | alice.agent@example.com | Agent |
| Bob Supervisor | bob.supervisor@example.com | Supervisor |
| Carol Admin | carol.admin@example.com | Admin |
| Dave Agent | dave.agent@example.com | Agent |

## Tickets

| Title | Status | Priority |
|-------|--------|----------|
| Cannot login to portal | Open | High |
| Printer not working | InProgress | Medium |
| Email sync delay | Resolved | Low |
| VPN connection drops | Closed | High |
| Request new monitor | Cancelled | Low |

Idempotent by email (users) and title (tickets).
