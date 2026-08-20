# INDTEC LABZ / 001 — Catalog

> A deliberately small music catalog and setlist service built to demonstrate production-oriented software design without hiding simple problems behind unnecessary abstractions.

`STATUS / EXPERIMENTAL` · `.NET 10` · `REST` · `DDD` · `PostgreSQL` · `Dapper` · `Docker`

## The problem

Musicians need a catalog of songs and a way to assemble setlists for live performances. A song has its original musical key, but the same song can be performed in a different key in a specific setlist.

The domain is intentionally familiar. The interesting part of this lab is not CRUD — it is where rules live, which abstractions earn their place, and how the service can evolve without turning into an architecture showcase for its own sake.

## Domain

```text
Music : IAggregateRoot<Guid>
├── name / artist
├── duration / bpm
└── original key

Setlist : IAggregateRoot<Guid>
├── status: Draft | Published
└── SetlistSong[]
    ├── position
    └── performance key
```

Current invariants:

- a music can appear only once in a setlist;
- a setlist must contain at least one music before publication;
- published setlists are immutable;
- removing a music closes the position gap;
- performance key belongs to the setlist entry, not to the catalog music.

## Architecture

```text
HTTP
 ↓
Api
 ↓
Application ─────→ Domain ←───── Infrastructure
                         ↓
                  repository contracts
```

Dependencies point inward. Domain owns the aggregate abstractions and repository contracts; infrastructure implements them. Application orchestrates use cases without knowing how aggregates are persisted.

The small domain building blocks use generic identity types:

```csharp
IEntity<TId>
IAggregateRoot<TId> : IEntity<TId>
IRepository<TAggregate, TId>
```

`Guid` is the identity choice for this service, not a framework rule. Another aggregate could use a sequential `long`, a strongly typed ID or another identity strategy without changing the building blocks.

## Engineering decisions

### Why repository contracts live in Domain

`IMusicRepository` and `ISetlistRepository` describe access to aggregate roots and are part of this domain model. Infrastructure depends on those contracts, not the opposite. Application-only dependencies such as external gateways, current-user context or notification clients would still belong at the application boundary.

### Why a marker `IRepository<TAggregate, TId>` but no generic CRUD repository

The marker communicates that repositories operate on aggregate roots. It intentionally exposes no CRUD methods. `IMusicRepository` and `ISetlistRepository` still declare only the operations their aggregates actually need.

### Why generic entity IDs

The building blocks should describe identity semantics without forcing `Guid`. The current aggregates use `Guid`; the abstraction remains open to sequential or strongly typed identifiers when the domain justifies them.

### Why Result for domain failures

Expected rule violations are represented as `Result` / `Result<T>` with stable error codes. An empty setlist or duplicate music is not exceptional infrastructure failure — it is a valid domain outcome that callers can handle explicitly.

### Why `TimeProvider`

Publication time is an external dependency. The Application layer reads time through .NET's `TimeProvider` and passes the timestamp into the Domain:

```text
TimeProvider
    ↓
Application
    ↓
setlist.Publish(now)
```

The Domain stays deterministic and unaware of clocks. Tests can provide a fixed `TimeProvider` and assert the exact publication timestamp without inventing a custom `IClock` abstraction.

### Why not split Music and Setlist into microservices?

They currently belong to the same small bounded context. Splitting them would introduce network calls, distributed consistency and operational overhead without solving an actual boundary problem. Other LABZ services will be extracted when an integration boundary gives us a reason to do it.

### Why Dapper?

SQL is useful information here, not an implementation detail we need to hide. Dapper keeps the mapping thin while allowing persistence and query decisions to remain explicit.

### Why is `SetlistPublished` only a domain result for now?

Nothing outside this service needs the event yet. Introducing a broker before a consumer exists would be speculative infrastructure. A future LABZ service can create that boundary and force the integration design naturally.

## Trade-offs

This lab deliberately does **not** start with MediatR, a generic CRUD repository, AutoMapper, a shared kernel, message brokers or a large hierarchy of base classes. Those tools can be useful, but none of them is a requirement for Clean Architecture, DDD or SOLID.

The rule for INDTEC LABZ is simple:

> **Patterns are evidence of a problem solved, not items on a checklist.**

## Local environment

PostgreSQL is available through Docker:

```bash
docker compose up -d
```

The database initialization script lives in `database/001_init.sql`.

## Roadmap

The first vertical slice is intentionally constrained:

```text
Create Music
→ Create Setlist
→ Add Music
→ Publish Setlist
→ Query Setlist
```

Once this flow is complete, new requirements should preferably become boundaries for another LABZ service instead of making Catalog indefinitely larger.

---

**INDTEC LABZ** is a portfolio engineering series. Each repository focuses on a small problem and makes the technical decisions, trade-offs and failure modes visible.