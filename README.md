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
| `RestaurantReservation.Console` | Console app that exercises every repository method against the real database |

The console app's assembly is `RestaurantReservation.Console`, but its root namespace stays
`RestaurantReservation`: inside a namespace ending in `.Console`, an unqualified `Console.WriteLine`
binds to the namespace instead of `System.Console` and fails to compile.

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

   Without the EF CLI installed, the console app can apply the same migrations itself:

   ```bash
   dotnet run --project RestaurantReservation/RestaurantReservation.Console -- --migrate
   ```

3. **(Optional) verify:**
   ```bash
   docker exec rr-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa \
     -P 'very_STRONG_password123' -C -d RestaurantReservationCore \
     -Q "SELECT COUNT(*) FROM Customers;"
   ```

The connection string lives in each project's `appsettings.json` (the Db copy is what the EF CLI reads
at design time) and matches the SA password in `docker-compose.yml`. Both honour a
`ConnectionStrings__DefaultConnection` environment variable as an override.

## Run the demo

```bash
dotnet run --project RestaurantReservation/RestaurantReservation.Console
```

Every repository method is called against the live database and its result printed. The run is
repeatable: each CRUD section inserts a row of its own, updates it, deletes it again, and leaves the
seed data untouched.

| Command | Effect |
| --- | --- |
| `dotnet run` | run every section |
| `dotnet run -- queries views` | run only the named sections |
| `dotnet run -- --sql` | also log the SQL EF Core generates |
| `dotnet run -- --migrate` | apply pending migrations first |
| `dotnet run -- --help` | list sections and options |

Sections are `crud` (requirement 9), `queries` (10), `views` (11), `function` (12) and `procedure` (13).
The app exits `0` on success, `1` if a section failed, `2` if the database is unreachable or
un-migrated, and `64` for a bad command line.
