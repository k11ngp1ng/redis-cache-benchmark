# Project guidance

## Purpose and proposed shape

Build a small, reproducible C# project that teaches how caching changes application performance, with Redis as the shared/distributed cache. The first implementation should compare the same deterministic data lookup under three paths:

1. No cache (baseline).
2. Process-local memory cache.
3. Redis cache.

Keep the benchmark harness separate from the example application logic so benchmark setup, data generation, and measured operations are easy to understand. Use a local Redis service for development (for example, Docker Compose), document how to run it, and make it possible to run the non-Redis paths without Redis.

Report more than a single average: capture throughput and latency distribution (at least median and tail latency where the chosen tooling supports it), plus cache hit/miss behavior. Keep benchmark parameters and environment details visible. Do not claim results from one machine as universal performance conclusions. Select the .NET target, Redis client, benchmark library, and exact workload with the user before the first implementation part; prefer supported stable releases and explain tradeoffs in the README.

## Work in small committed parts

The user wants each project part delivered as its own commit. For every requested implementation/update:

- Inspect `git status` and the current branch before editing. Preserve unrelated user changes.
- Make one coherent, reviewable part per update. Avoid mixing unrelated cleanup or future parts into it.
- Run only the checks appropriate to that part and requested by the user; report what ran and what did not.
- Review the diff, stage only files belonging to that part, and create one descriptive commit for that part.
- Do not amend, squash, or rewrite earlier commits. Do not push unless the user asks.
- If a change cannot be committed (for example, missing Git identity or a conflict), leave the work intact and explain the exact blocker.

## Project boundaries

- Keep the sample understandable to a C# learner. Prefer a small solution with clear names and focused projects over premature abstraction.
- Keep Redis connection details configurable; do not commit secrets or machine-specific configuration.
- Make benchmark runs repeatable and distinguish warm-cache from cold-cache behavior. Do not compare unlike operations or include one-time setup in the measured operation unless explicitly studying startup cost.
- Document prerequisites, startup/shutdown, commands, benchmark scenarios, and interpretation of results.
- Update this guidance when the user changes the project direction or workflow.
