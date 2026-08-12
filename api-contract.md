# API Contract

Base URL: `http://localhost:5xxx/api` (port configured at launch; documented in README)

Content-Type: `application/json`

Error format: [RFC 7807 Problem Details](https://tools.ietf.org/html/rfc7807) or consistent JSON:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Title": ["The Title field is required."]
  }
}
```

---

## Users

### List users (for dropdowns)

**Method:** `GET`  
**Path:** `/api/users`  
**Purpose:** Return all seeded users for CreatedBy/AssignedTo selection.

#### Response `200 OK`

```json
[
  {
    "id": 1,
    "name": "Alice Agent",
    "email": "alice.agent@example.com",
    "role": "Agent"
  }
]
```

#### Error Responses

| Status | Condition |
|--------|-----------|
| 500 | Server/database error |

---

## Tickets

### List tickets (with search and filter)

**Method:** `GET`  
**Path:** `/api/tickets`  
**Purpose:** List tickets with optional keyword search and status filter.

#### Query Parameters

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `search` | string | No | Keyword search in title and description (case-insensitive contains) |
| `status` | string | No | Exact status filter: `Open`, `InProgress`, `Resolved`, `Closed`, `Cancelled` |

#### Response `200 OK`

```json
[
  {
    "id": 1,
    "title": "Cannot login to portal",
    "description": "User reports 401 after password reset.",
    "priority": "High",
    "status": "Open",
    "assignedTo": { "id": 4, "name": "Dave Agent" },
    "createdBy": { "id": 1, "name": "Alice Agent" },
    "createdAt": "2026-08-11T08:00:00Z",
    "updatedAt": "2026-08-11T08:00:00Z",
    "commentCount": 2
  }
]
```

**Default sort:** `UpdatedAt` descending.

#### Error Responses

| Status | Condition |
|--------|-----------|
| 400 | Invalid status filter value |

---

### Get ticket by ID

**Method:** `GET`  
**Path:** `/api/tickets/{id}`  
**Purpose:** Retrieve full ticket detail including comments.

#### Response `200 OK`

```json
{
  "id": 1,
  "title": "Cannot login to portal",
  "description": "User reports 401 after password reset.",
  "priority": "High",
  "status": "Open",
  "assignedTo": { "id": 4, "name": "Dave Agent", "email": "dave.agent@example.com" },
  "createdBy": { "id": 1, "name": "Alice Agent", "email": "alice.agent@example.com" },
  "createdAt": "2026-08-11T08:00:00Z",
  "updatedAt": "2026-08-11T08:00:00Z",
  "comments": [
    {
      "id": 1,
      "message": "Reset link expired — sending new one.",
      "createdBy": { "id": 4, "name": "Dave Agent" },
      "createdAt": "2026-08-11T09:15:00Z"
    }
  ],
  "validNextStatuses": ["InProgress", "Cancelled"]
}
```

#### Error Responses

| Status | Condition |
|--------|-----------|
| 404 | Ticket not found |

---

### Create ticket

**Method:** `POST`  
**Path:** `/api/tickets`  
**Purpose:** Create a new ticket with status `Open`.

#### Request

```json
{
  "title": "New support request",
  "description": "Detailed description of the issue.",
  "priority": "Medium",
  "createdById": 1,
  "assignedToId": 4
}
```

#### Validation Rules

| Field | Rules |
|-------|-------|
| `title` | Required, max 200 characters |
| `description` | Required, max 4000 characters |
| `priority` | Required, one of: `Low`, `Medium`, `High` |
| `createdById` | Required, must exist in Users |
| `assignedToId` | Required, must exist in Users |

#### Response `201 Created`

Returns created ticket (same shape as detail, without comments or with empty comments).

`Location` header: `/api/tickets/{id}`

#### Error Responses

| Status | Condition |
|--------|-----------|
| 400 | Validation failure |
| 400 | User ID not found |

---

### Update ticket

**Method:** `PUT`  
**Path:** `/api/tickets/{id}`  
**Purpose:** Update ticket fields. **Does not change status.**

#### Request

```json
{
  "title": "Updated title",
  "description": "Updated description.",
  "priority": "High",
  "assignedToId": 2
}
```

#### Validation Rules

Same as create (except `createdById` is not updatable).

#### Response `200 OK`

Returns updated ticket detail.

#### Error Responses

| Status | Condition |
|--------|-----------|
| 400 | Validation failure |
| 404 | Ticket not found |
| 400 | AssignedTo user not found |

---

### Change ticket status

**Method:** `POST`  
**Path:** `/api/tickets/{id}/status`  
**Purpose:** Transition ticket status via enforced state machine.

#### Request

```json
{
  "status": "InProgress"
}
```

#### Validation Rules

| Field | Rules |
|-------|-------|
| `status` | Required, valid enum value |
| Transition | Must be allowed by state machine from current status |

#### Response `200 OK`

Returns updated ticket with new status and refreshed `validNextStatuses`.

#### Error Responses

| Status | Condition |
|--------|-----------|
| 400 | Invalid target status value |
| 400 / 422 | Invalid transition (e.g., "Cannot transition from Open to Resolved") |
| 404 | Ticket not found |

**Example invalid transition response:**

```json
{
  "type": "about:blank",
  "title": "Invalid status transition",
  "status": 400,
  "detail": "Cannot transition from 'Open' to 'Resolved'. Allowed transitions from 'Open': InProgress, Cancelled."
}
```

---

### Get valid next statuses (optional helper)

**Method:** `GET`  
**Path:** `/api/tickets/{id}/valid-transitions`  
**Purpose:** Return allowed next statuses for UI. Backend logic shared with status change endpoint.

#### Response `200 OK`

```json
{
  "currentStatus": "Open",
  "validNextStatuses": ["InProgress", "Cancelled"]
}
```

#### Error Responses

| Status | Condition |
|--------|-----------|
| 404 | Ticket not found |

---

## Comments

### Add comment to ticket

**Method:** `POST`  
**Path:** `/api/tickets/{ticketId}/comments`  
**Purpose:** Append a comment to a ticket.

#### Request

```json
{
  "message": "Customer confirmed issue is resolved.",
  "createdById": 1
}
```

#### Validation Rules

| Field | Rules |
|-------|-------|
| `message` | Required, max 2000 characters |
| `createdById` | Required, must exist in Users |

#### Response `201 Created`

```json
{
  "id": 5,
  "message": "Customer confirmed issue is resolved.",
  "createdBy": { "id": 1, "name": "Alice Agent" },
  "createdAt": "2026-08-11T10:30:00Z"
}
```

#### Error Responses

| Status | Condition |
|--------|-----------|
| 400 | Validation failure |
| 404 | Ticket not found |
| 400 | User not found |

---

## Endpoint Summary

| Method | Path | Purpose |
|--------|------|---------|
| GET | `/api/users` | List seeded users |
| GET | `/api/tickets` | List tickets (search, filter) |
| GET | `/api/tickets/{id}` | Ticket detail + comments |
| POST | `/api/tickets` | Create ticket |
| PUT | `/api/tickets/{id}` | Update ticket fields |
| POST | `/api/tickets/{id}/status` | Change status (state machine) |
| GET | `/api/tickets/{id}/valid-transitions` | Valid next statuses (UI helper) |
| POST | `/api/tickets/{ticketId}/comments` | Add comment |

---

## HTTP Status Code Conventions

| Code | Usage |
|------|-------|
| 200 | Successful GET, PUT, status change |
| 201 | Successful POST (create) |
| 400 | Validation errors, invalid transition, bad FK |
| 404 | Resource not found |
| 500 | Unhandled server error |

---

## Swagger

All endpoints documented via Swashbuckle in Development:

- `GET /swagger` — Swagger UI
- OpenAPI JSON at `/swagger/v1/swagger.json`

---

## Design Decisions

1. **Separate status endpoint** — prevents accidental status change via general update; keeps state machine enforcement explicit.
2. **`validNextStatuses` in detail response** — reduces round trips for UI; optional dedicated endpoint also available.
3. **User summary objects in responses** — avoids exposing full user entity; sufficient for UI display.
4. **Search via query string** — RESTful filtering on collection resource.
5. **No DELETE endpoints** — not required in Core scope.
