# Data Model

## Entity Relationship Diagram

```
┌─────────────────┐         ┌─────────────────────────────┐
│      User       │         │           Ticket             │
├─────────────────┤         ├─────────────────────────────┤
│ Id (PK)         │◄───┐    │ Id (PK)                     │
│ Name            │    │    │ Title                       │
│ Email (unique)  │    ├────│ CreatedById (FK → User)     │
│ Role            │    │    │ AssignedToId (FK → User)    │
└─────────────────┘    │    │ Description                 │
        ▲              │    │ Priority (enum)             │
        │              │    │ Status (enum)               │
        │              │    │ CreatedAt                   │
        │              │    │ UpdatedAt                   │
        │              │    └──────────────┬──────────────┘
        │              │                   │
        │              │    ┌──────────────▼──────────────┐
        │              │    │          Comment             │
        │              │    ├─────────────────────────────┤
        │              └────│ CreatedById (FK → User)     │
        │                   │ Id (PK)                     │
        └───────────────────│ TicketId (FK → Ticket)      │
                            │ Message                     │
                            │ CreatedAt                   │
                            └─────────────────────────────┘
```

---

## Entities

### User

| Column | Type | Constraints | Notes |
|--------|------|-------------|-------|
| Id | int | PK, identity | |
| Name | nvarchar(100) | NOT NULL | |
| Email | nvarchar(256) | NOT NULL, UNIQUE | Seed key for idempotency |
| Role | nvarchar(50) | NOT NULL | e.g., Agent, Supervisor, Admin |

**Relationships:**
- One User → many Tickets (as creator)
- One User → many Tickets (as assignee)
- One User → many Comments (as creator)

**Core behavior:** Seeded only; no CRUD API except read-all for UI dropdowns.

---

### Ticket

| Column | Type | Constraints | Notes |
|--------|------|-------------|-------|
| Id | int | PK, identity | |
| Title | nvarchar(200) | NOT NULL | |
| Description | nvarchar(4000) | NOT NULL | |
| Priority | nvarchar(20) | NOT NULL | Enum: Low, Medium, High |
| Status | nvarchar(20) | NOT NULL | Enum: see below |
| CreatedById | int | FK → User, NOT NULL | |
| AssignedToId | int | FK → User, NOT NULL | |
| CreatedAt | datetime2 | NOT NULL | UTC, server-set |
| UpdatedAt | datetime2 | NOT NULL | UTC, server-set |

**Default on create:** `Status = Open`

**Indexes (proposed):**
- `IX_Tickets_Status` — filter by status
- `IX_Tickets_UpdatedAt` — default sort
- `IX_Tickets_Title` — optional, supports search (LIKE queries)

**Relationships:**
- Many Tickets → one User (CreatedBy)
- Many Tickets → one User (AssignedTo)
- One Ticket → many Comments

---

### Comment

| Column | Type | Constraints | Notes |
|--------|------|-------------|-------|
| Id | int | PK, identity | |
| TicketId | int | FK → Ticket, NOT NULL | |
| Message | nvarchar(2000) | NOT NULL | |
| CreatedById | int | FK → User, NOT NULL | |
| CreatedAt | datetime2 | NOT NULL | UTC, server-set |

**Indexes:**
- `IX_Comments_TicketId` — load comments for ticket detail

**Delete behavior:**
- Ticket delete: **Restrict** (no ticket delete API in Core; preserve referential integrity)
- User delete: **Restrict** (users are seeded reference data)

---

## Enumerations

### TicketStatus

| Value | Description |
|-------|-------------|
| `Open` | Initial state on create |
| `InProgress` | Work started |
| `Resolved` | Fix applied, pending closure |
| `Closed` | Terminal — completed |
| `Cancelled` | Terminal — abandoned |

### Valid Transitions

```
Open        → InProgress, Cancelled
InProgress  → Resolved, Cancelled
Resolved    → Closed
Closed      → (none)
Cancelled   → (none)
```

### Priority

| Value |
|-------|
| `Low` |
| `Medium` |
| `High` |

### UserRole (informational)

| Value |
|-------|
| `Agent` |
| `Supervisor` |
| `Admin` |

---

## Seed Data Plan

### Users (deterministic, idempotent by Email)

| Name | Email | Role |
|------|-------|------|
| Alice Agent | alice.agent@example.com | Agent |
| Bob Supervisor | bob.supervisor@example.com | Supervisor |
| Carol Admin | carol.admin@example.com | Admin |
| Dave Agent | dave.agent@example.com | Agent |

### Sample Tickets (idempotent by deterministic title or seed flag)

| Title | Status | Priority | CreatedBy | AssignedTo |
|-------|--------|----------|-----------|------------|
| Cannot login to portal | Open | High | Alice | Dave |
| Printer not working | InProgress | Medium | Bob | Alice |
| Email sync delay | Resolved | Low | Carol | Dave |
| VPN connection drops | Closed | High | Alice | Bob |
| Request new monitor | Cancelled | Low | Dave | Alice |

### Sample Comments

- 2–3 comments on active tickets (Open, InProgress, Resolved)
- Linked to seeded users

### Idempotency approach

```csharp
if (!context.Users.Any(u => u.Email == "alice.agent@example.com"))
    context.Users.Add(...);
await context.SaveChangesAsync();
```

Or use fixed IDs in seed with `HasData` in EF configuration (migration-based seed).

---

## EF Core Configuration Notes

- Store enums as **strings** via `.HasConversion<string>()`
- Configure FK relationships in `OnModelCreating` or `IEntityTypeConfiguration<T>`
- `UpdatedAt` updated in service layer on every ticket modify
- Migrations output to `SupportTicket.Infrastructure/Migrations/`
- Copy/reference migration SQL in `database/schema-or-migrations/` for documentation

---

## Database Connection

```
Server=localhost\SQLEXPRESS;Database=SupportTicketDb;Trusted_Connection=True;TrustServerCertificate=True;
```

- Database created automatically on first migration if not exists
- **Not** using `master`, LocalDB, SQLite, or in-memory for persistence

---

## SSMS Verification Checklist (Phase 4)

- [ ] Database `SupportTicketDb` exists
- [ ] Tables: `Users`, `Tickets`, `Comments`
- [ ] FK constraints: `Tickets.CreatedById`, `Tickets.AssignedToId`, `Comments.TicketId`, `Comments.CreatedById`
- [ ] Unique index on `Users.Email`
- [ ] Seed rows present
- [ ] Enum values readable as strings in `Status`, `Priority`, `Role` columns
