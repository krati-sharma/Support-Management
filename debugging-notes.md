# Debugging Notes

## Issue 1 — `dotnet ef` tool missing

### Problem
`dotnet ef migrations add` failed because the EF CLI tool was not installed.

### How I Investigated
CLI error indicated `dotnet-ef` was not found on PATH.

### How AI Helped
Suggested installing the matching EF Core 8 global tool.

### What I Validated
Installed `dotnet-ef` 8.0.11 and successfully created/applied `InitialCreate`.

### Final Fix
```powershell
dotnet tool install --global dotnet-ef --version 8.0.11
```

---

## Issue 2 — Frontend scaffolded in wrong folder

### Problem
Vite app was initially created at repo root `support-ticket-ui/` instead of `src/frontend/support-ticket-ui/`.

### How I Investigated
Directory listing showed missing `src/frontend` when `npm create vite` ran before folder creation finished.

### How AI Helped
Moved the project into the required assessment structure.

### What I Validated
`package.json` and `node_modules` present under `src/frontend/support-ticket-ui/`.

### Final Fix
Moved folder to `src/frontend/support-ticket-ui` and installed `react-router-dom`.

---

## Issue 3 — Integration test `UseEnvironment` compile error

### Problem
Test project failed to compile: `IWebHostBuilder` had no `UseEnvironment` extension.

### How I Investigated
Missing ASP.NET Core shared framework reference on the xUnit project.

### How AI Helped
Added `FrameworkReference` to `Microsoft.AspNetCore.App` and switched DB replacement to `ConfigureTestServices`.

### What I Validated
Tests compiled and 15/15 passed.

### Final Fix
Updated `SupportTicket.IntegrationTests.csproj` + factory setup.

---

## Issue 4 — TypeScript `verbatimModuleSyntax` FormEvent import

### Problem
Frontend build failed: `FormEvent` must be a type-only import.

### How I Investigated
`tsc -b` errors on create/detail pages.

### How AI Helped
Changed to `import type { FormEvent } from 'react'`.

### What I Validated
`npm run build` succeeded.

### Final Fix
Type-only imports in `CreateTicketPage.tsx` and `TicketDetailPage.tsx`.

---

## Issue 5 — File lock during parallel `dotnet build`

### Problem
Transient MSB3713/CS2012 file-in-use errors when builds overlapped.

### How I Investigated
Another process held `obj` outputs.

### How AI Helped
Retried build after short wait.

### What I Validated
Subsequent `dotnet build` succeeded with 0 errors.

### Final Fix
Re-run build; no code change required.
