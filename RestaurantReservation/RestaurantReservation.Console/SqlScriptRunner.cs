namespace RestaurantReservation;

/// <summary>Executes one SQL file through <see cref="ISqlScriptExecutor" /> and prints the outcome.</summary>
public sealed class SqlScriptRunner(IServiceScopeFactory scopes)
{
    public async Task<int> RunAsync(string path, TimeSpan? commandTimeout, CancellationToken cancellationToken)
    {
        var fullPath = Path.GetFullPath(path);

        if (!File.Exists(fullPath))
        {
            Output.Problem($"SQL file not found: {fullPath}");

            return 4;
        }

        var sql = await File.ReadAllTextAsync(fullPath, cancellationToken);
        var name = Path.GetFileName(fullPath);

        using var scope = scopes.CreateScope();
        var executor = scope.ServiceProvider.GetRequiredService<ISqlScriptExecutor>();

        Output.Step($"ExecuteAsync({name})");

        var result = await executor.ExecuteAsync(sql, name, commandTimeout, cancellationToken);

        Output.Show(result, outcome => $"{outcome.RowsAffected} row(s) affected");

        return result.IsSuccess ? 0 : 5;
    }
}
