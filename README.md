# Restaurant Reservation ORM

A restaurant-reservation data layer built with **.NET 10** and **Entity Framework Core 10** (SQL Server).
It models restaurants, tables, employees, menu items, customers, reservations, orders and order items,
and exposes them through a repository layer — including database **views**, a scalar **function**, and a
**stored procedure**, all created via EF Core migrations. The schema follows the ER diagram in
[`diagrams/ER_Diagram.png`](diagrams/ER_Diagram.png).

## Projects

| Project | Contents |
| --- | --- |
| `RestaurantReservation.Db` | DbContext, entities, configurations, migrations + seed data, the view/function/procedure SQL, and one repository per entity |
| `RestaurantReservation.Tests` | Model tests and container-backed integration tests for the Db project |

## Tech stack

- .NET 10 / C#
- EF Core 10 (SQL Server provider)
- SQL Server 2025 — run locally via Docker Compose

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/)
- Docker (for the SQL Server container)
- EF Core CLI:
  ```bash
  dotnet tool install --global dotnet-ef --version 10.0.10
  ```
  Ensure `~/.dotnet/tools` is on your `PATH` (e.g. add `export PATH="$PATH:$HOME/.dotnet/tools"` to `~/.bashrc`).

## Setup

1. **Start SQL Server** (from the repo root), then wait until it reports `healthy`:

   ```bash
   docker compose up -d
   docker compose ps
   ```

2. **Create the database** (from the Db project folder):

   ```bash
   cd RestaurantReservation/RestaurantReservation.Db
   dotnet ef database update
   ```

   This creates `RestaurantReservationCore`, applies the schema and seed data, and creates the views,
   function and stored procedure.

3. **(Optional) verify:**
   ```bash
   docker exec rr-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa \
     -P 'very_STRONG_password123' -C -d RestaurantReservationCore \
     -Q "SELECT COUNT(*) FROM Customers;"
   ```

The connection string is in `RestaurantReservation/RestaurantReservation.Db/appsettings.json` and matches
the SA password in `docker-compose.yml`.

## Tests

`RestaurantReservation.Tests` targets the `RestaurantReservation.Db` project on two levels:

| Folder | Covers | Needs Docker |
| --- | --- | --- |
| `Model/` | What the configurations declare — keys, columns, precision, relationships and delete behaviour, view mappings, the foreign-key integrity of the `HasData` seed, and the DI registrations | no |
| `Integration/` | What SQL Server actually does — repository CRUD and every query method, both views, the scalar function, the stored procedure, restrict/cascade behaviour, and the objects the migration creates | yes |

```bash
dotnet test                                  # everything
dotnet test --filter Category=Model          # fast, no Docker
dotnet test --filter Category=Integration    # container-backed
```

The integration tests start **one** SQL Server 2025 container via
[Testcontainers](https://dotnet.testcontainers.org/), run `Database.MigrateAsync()` against it — so
the migration itself is under test, schema, seed, views, function and procedure included — and dispose
the container when the run ends. The Docker daemon has to be running; nothing else is required, and
the `docker compose` instance used for development is neither needed nor touched.

Each test gets its own `DbContext` inside a transaction that is rolled back on completion, so tests can
insert, update and delete freely while leaving the seeded database untouched for the next one. When a
test needs data of its own, `DatabaseTest.AddReservationAsync()` inserts a whole restaurant → table /
employee / menu item → reservation → order → order item chain in a single call.
