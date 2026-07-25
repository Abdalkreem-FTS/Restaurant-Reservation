using RestaurantReservation.Demos;

namespace RestaurantReservation;

public sealed class DemoRunner(Func<RestaurantReservationDbContext> newContext)
{
    private const int Success = 0;
    private const int DemoFailed = 1;
    public const int DatabaseUnavailable = 2;
    private const int InvalidArguments = 64;

    private readonly Section[] _sections =
    [
        new("crud", "Create / Update / Delete - every entity", new CrudDemo(newContext).RunAsync),
        new("queries", "Query methods", new QueryDemo(newContext).RunAsync),
        new("views", "Database views", new ViewDemo(newContext).RunAsync),
        new("function", "Database function - fn_CalculateRestaurantRevenue", new FunctionDemo(newContext).RunAsync),
        new("procedure", "Stored procedure - sp_FindCustomersByPartySize", new StoredProcedureDemo(newContext).RunAsync)
    ];
    
    private sealed record Section(string Key, string Title, Func<SampleIds, CancellationToken, Task> RunAsync);

    public async Task<int> RunAsync(DemoOptions options, CancellationToken cancellationToken)
    {
        if (options.HelpRequested)
        {
            PrintUsage();

            return Success;
        }

        if (options.UnknownFlags.Count > 0)
        {
            ConsoleWriter.Error($"Unknown option(s): {string.Join(", ", options.UnknownFlags)}");
            PrintUsage();

            return InvalidArguments;
        }

        var selected = SelectSections(options.Sections);

        if (selected is null)
        {
            return InvalidArguments;
        }

        if (!await PrepareDatabaseAsync(options.ApplyMigrations, cancellationToken))
        {
            return DatabaseUnavailable;
        }

        SampleIds ids;

        await using (var context = newContext())
        {
            ids = await SampleIds.LoadAsync(context, cancellationToken);
        }

        var failures = new List<string>();

        for (var index = 0; index < selected.Count; index++)
        {
            var section = selected[index];

            ConsoleWriter.Section(index + 1, section.Title);

            try
            {
                await section.RunAsync(ids, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                ConsoleWriter.Warning("Cancelled.");

                return DemoFailed;
            }
            catch (Exception exception)
            {
                failures.Add(section.Title);
                ConsoleWriter.Error($"{section.Title} failed: {exception.Message}");
            }

            ConsoleWriter.Line();
        }

        return Summarize(selected.Count, failures);
    }

    private IReadOnlyList<Section>? SelectSections(IReadOnlyList<string> keys)
    {
        if (keys.Count == 0)
        {
            return _sections;
        }

        var unknown = keys.Where(key => _sections.All(section => section.Key != key)).ToList();

        if (unknown.Count == 0)
        {
            return _sections.Where(section => keys.Contains(section.Key)).ToList();
        }

        ConsoleWriter.Error($"Unknown section(s): {string.Join(", ", unknown)}");
        PrintUsage();

        return null;
    }

    private async Task<bool> PrepareDatabaseAsync(bool applyMigrations, CancellationToken cancellationToken)
    {
        await using var context = newContext();

        try
        {
            if (applyMigrations)
            {
                ConsoleWriter.Warning("Applying migrations...");
                await context.Database.MigrateAsync(cancellationToken);
                ConsoleWriter.Warning("Migrations applied.");
                ConsoleWriter.Line();

                return true;
            }

            if (!await context.Database.CanConnectAsync(cancellationToken))
            {
                ConsoleWriter.Error("Cannot reach the RestaurantReservationCore database.");
                PrintDatabaseHints();

                return false;
            }

            var pending = (await context.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();

            if (pending.Count == 0)
            {
                return true;
            }

            ConsoleWriter.Error($"The database is missing {pending.Count} migration(s): {string.Join(", ", pending)}");
            PrintDatabaseHints();

            return false;
        }
        catch (OperationCanceledException)
        {
            ConsoleWriter.Warning("Cancelled.");

            return false;
        }
        catch (Exception exception)
        {
            ConsoleWriter.Error($"Database check failed: {exception.Message}");
            PrintDatabaseHints();

            return false;
        }
    }

    private static int Summarize(int sectionCount, List<string> failures)
    {
        if (failures.Count == 0)
        {
            ConsoleWriter.Line($"All {sectionCount} section(s) completed successfully.");

            return Success;
        }

        ConsoleWriter.Error($"{failures.Count} of {sectionCount} section(s) failed: {string.Join(", ", failures)}");

        return DemoFailed;
    }

    private void PrintUsage()
    {
        ConsoleWriter.Line();
        ConsoleWriter.Line("Usage: dotnet run --project RestaurantReservation.Console [--] [sections...] [options]");
        ConsoleWriter.Line();
        ConsoleWriter.Line("Sections (default: all, in this order):");

        foreach (var section in _sections)
        {
            ConsoleWriter.Line($"  {section.Key,-12}{section.Title}");
        }

        ConsoleWriter.Line();
        ConsoleWriter.Line("Options:");
        ConsoleWriter.Line("  --migrate   apply pending migrations before running");
        ConsoleWriter.Line("  --sql       log the SQL that EF Core generates");
        ConsoleWriter.Line("  --help, -h  show this help");
        ConsoleWriter.Line();
    }

    private static void PrintDatabaseHints()
    {
        ConsoleWriter.Line();
        ConsoleWriter.Line("  Fix it with:");
        ConsoleWriter.Line("    1. start SQL Server:     docker compose up -d");
        ConsoleWriter.Line("    2. create the database:  dotnet ef database update");
        ConsoleWriter.Line("       (or, without the EF CLI installed:  dotnet run -- --migrate)");
        ConsoleWriter.Line();
    }
}
