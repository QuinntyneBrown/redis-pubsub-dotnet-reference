# Detailed Designs — Index

Reference application demonstrating Redis Pub/Sub patterns across .NET microservices, with a small common library that keeps message-handling boilerplate out of every service.

| # | Feature | Status | Description |
|---|---------|--------|-------------|
| 01 | [Messaging Library (RedisBus)](01-messaging-library/README.md) | Complete | Radically-simple common library for telemetry, events, query/response, and request/response over Redis Pub/Sub |
| 02 | [Reference Application — Smart Building](02-reference-application/README.md) | Complete | Six .NET microservices that exemplify each pattern using the library |
| 03 | [Command Pattern Extension](03-command-pattern/README.md) | Complete | Adds `ICommand<TResponse>` (channel `cmd.*`) as a fifth pattern alongside telemetry / event / query / request |
| 04 | [Backend-for-Frontend (BFF) Service](04-bff-service/README.md) | Draft | Single frontend door — HTTP for command/query/request + SignalR hub for selective subscription and push |
| 05 | [Frontend Dashboard — Angular](05-frontend-dashboard/README.md) | Draft | Dark-themed Angular Material SPA with Chart.js line charts, Material tables, and a thermostat command form |

## Reading order

Read **01** first for the library primitives, then **02** for the six-service smart-building application that uses them. **03** adds the command pattern to the library. **04** introduces the BFF that fronts the system for browsers. **05** is the Angular SPA that consumes **04**.
