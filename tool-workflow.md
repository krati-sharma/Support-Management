# Tool Workflow

## Primary AI Tool

**Cursor AI — Agent mode**

This assessment uses Cursor as the sole populated entry under `tool-specific/`. Other tool folders (`kiro-specs/`, `other-tool-workflow/`) remain empty per instructions.

## Workflow Overview

```
Requirements → Design → Setup → Database → Backend → Tests → Frontend → E2E → Review → Docs
     ↑              ↑         ↑        ↑         ↑        ↑         ↑       ↑       ↑       ↑
  Human review at each gate; AI assists with drafting, scaffolding, and iteration
```

## How AI Is Used

| Activity | AI role | Human role |
|----------|---------|------------|
| Planning | Analyze requirements, draft artifacts | Review assumptions, approve plan |
| Design | Propose architecture, API, data model | Validate against requirements |
| Implementation | Generate code incrementally | Build, run, inspect, fix |
| Testing | Draft test cases | Run tests, record real results |
| Debugging | Suggest fixes | Reproduce, validate fixes |
| Code review | AI review suggestions | Accept/reject selectively |
| Documentation | Draft README, summaries | Ensure accuracy |

## Iteration Pattern

1. Provide context (requirements, current file state)
2. AI proposes approach or code
3. Human validates via build/test/browser/SSMS
4. Accept, modify, or reject
5. Record decision in `ai-prompts/{activity}.md`

## Validation Discipline

- Do not fabricate test results, debugging incidents, or reflections
- Mark personal fields (candidate name, reflection) for manual completion
- Run builds and tests before claiming success
- Verify database in SSMS after migrations

## Cursor-Specific Practices

See [`tool-specific/cursor-workflow/`](tool-specific/cursor-workflow/) for:
- Agent mode usage notes
- Phase-gated implementation approach
- Prompt history conventions

## Git Workflow

- Local Git repository (no push to GitHub unless explicitly requested)
- Meaningful commits per phase/milestone
- `.gitignore` excludes secrets, `node_modules`, build output

## Prompt History

Maintained under `ai-prompts/` grouped by activity:
- `planning.md` — requirements and planning prompts
- `design.md` — architecture and design prompts
- `implementation.md` — coding prompts
- `testing.md` — test-related prompts
- `debugging.md` — troubleshooting prompts
- `code-review.md` — review prompts
- `documentation.md` — docs prompts

Each entry captures: prompt, AI summary, accepted/modified/rejected, why, and manual validation.
