# Restaurant Reservation ORM

A restaurant-reservation data layer built with **.NET 10** and **Entity Framework Core 10** (SQL Server).
It models restaurants, tables, employees, menu items, customers, reservations, orders and order items,
and exposes them through a repository layer — including database **views**, a scalar **function**, and a
**stored procedure**, all created via EF Core migrations. The schema follows the ER diagram in
[`diagrams/ER_Diagram.png`](diagrams/ER_Diagram.png).

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
