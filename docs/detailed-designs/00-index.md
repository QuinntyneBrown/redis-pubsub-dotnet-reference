# Detailed Designs — Index

Reference application demonstrating Redis Pub/Sub patterns across .NET microservices, with a small common library that keeps message-handling boilerplate out of every service.

| # | Feature | Status | Description |
|---|---------|--------|-------------|
| 01 | [Messaging Library (RedisBus)](01-messaging-library/README.md) | Complete | Radically-simple common library for telemetry, events, query/response, and request/response over Redis Pub/Sub |
| 02 | [Reference Application — Smart Building](02-reference-application/README.md) | Complete | Six .NET microservices that exemplify each pattern using the library |

## Reading order

Read **01** first to understand the library primitives. Read **02** to see them composed across six services and four message patterns.
