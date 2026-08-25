# Restaurant Reservation

A restaurant-reservation system built with **.NET 10**, in two parts: a data layer over SQL Server
built with **EF Core 10**, and an HTTP API on top of it.

It models restaurants, tables, employees, menu items, customers, reservations, orders and order items,
and exposes them through a repository layer — including database **views**, a scalar **function**, and a
**stored procedure**, all created via EF Core migrations. The schema follows the ER diagram in
[`diagrams/ER_Diagram.png`](diagrams/ER_Diagram.png).

## Projects

| | |
|---|---|
| [`RestaurantReservation.Db`](RestaurantReservation/RestaurantReservation.Db) | Entities, migrations, seed data and repositories |
| [`RestaurantReservation.API`](RestaurantReservation/RestaurantReservation.API) | Minimal-API HTTP front end with JWT auth |
| [`RestaurantReservation.Console`](RestaurantReservation/RestaurantReservation.Console) | Demo app that exercises the repositories |
| [`RestaurantReservation.Tests`](RestaurantReservation/RestaurantReservation.Tests) | Integration tests against a real SQL Server |
| [`load/`](load/README.md) | Drives the console app hard enough to fill Kibana with plausible traffic |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/)
- Docker
- EF Core CLI:
  ```bash
  dotnet tool install --global dotnet-ef --version 10.0.10
  ```
  Ensure `~/.dotnet/tools` is on your `PATH` (e.g. add `export PATH="$PATH:$HOME/.dotnet/tools"` to `~/.bashrc`).

## Setup

1. **Start the containers** — SQL Server, Redis, Elasticsearch and Kibana — then wait until they report
   `healthy`:

   ```bash
   docker compose up -d
   docker compose ps
   ```

2. **Create the database:**

   ```bash
   dotnet ef database update --project RestaurantReservation/RestaurantReservation.Db
   ```

   This creates `RestaurantReservationCore`, applies the schema and seed data, and creates the views,
   function and stored procedure.

3. **Run the API:**

   ```bash
   dotnet run --project RestaurantReservation/RestaurantReservation.API
   ```

   Swagger is at <http://localhost:5122/swagger>, and
   [`RestaurantReservation.API.http`](RestaurantReservation/RestaurantReservation.API/RestaurantReservation.API.http)
   walks every endpoint in order.

   Sign in at `POST /api/tokens`. Every seeded user has the password `Password123!` — `alice.turner`
   is a manager, `bob.cook` a waiter, `john.doe` a customer. Staff see every record; everybody else
   sees only their own.

4. **Run the tests:**

   ```bash
   dotnet test RestaurantReservation/RestaurantReservation.slnx
   ```

Connection strings live in each project's `appsettings.json` and match the passwords and ports in
`docker-compose.yml`.
