# Redis Pub/Sub .NET Reference

A small reference application demonstrating four Redis Pub/Sub messaging patterns in .NET microservices using a tiny shared library, **RedisBus**.

## Patterns

| Pattern | Direction | Marker | Channel |
|---|---|---|---|
| Telemetry | one-way fan-out | `ITelemetry` | `tel.*` |
| Event | one-way fan-out | `IEvent` | `evt.*` |
| Query / response | request → reply (read) | `IQuery<TResponse>` | `qry.*` → `rep.*` |
| Request / response | request → reply (write) | `IRequest<TResponse>` | `req.*` → `rep.*` |

## Layout

- `src/RedisBus/` — the shared messaging library
- `tests/RedisBus.UnitTests/` — fast in-process tests
- `tests/RedisBus.IntegrationTests/` — Testcontainers-backed end-to-end tests (require Docker)
- `docs/detailed-designs/` — per-feature design docs

See `docs/folder-structure.md` for the full layout and `docs/detailed-designs/00-index.md` for the design index.

## Build and test

```bash
dotnet build
dotnet test tests/RedisBus.UnitTests/RedisBus.UnitTests.csproj
dotnet test tests/RedisBus.IntegrationTests/RedisBus.IntegrationTests.csproj   # needs Docker
```
