namespace RestaurantReservation;

/// <summary>Console formatting, kept to the few shapes the demo actually prints.</summary>
public static class Output
{
    public static void Title(string text)
    {
        Console.WriteLine();
        Write(ConsoleColor.Cyan, $"=== {text} ===");
    }

    /// <summary>The call about to be made, written the way it appears in code.</summary>
    public static void Step(string call)
    {
        Console.WriteLine();
        Write(ConsoleColor.White, $"  > {call}");
    }

    /// <summary>A result, printed as its value or as the error it came back with.</summary>
    public static void Show<T>(Result<T> result, Func<T, string> describe)
    {
        if (result.IsSuccess)
        {
            Write(ConsoleColor.Green, $"    ok     {describe(result.Value)}");
        }
        else
        {
            Write(ConsoleColor.Red, $"    error  {result.TopError.Code} - {result.TopError.Description}");
        }
    }

    public static void Value(string label, string value) => Console.WriteLine($"    {label}: {value}");

    public static void Note(string text) => Write(ConsoleColor.DarkGray, $"    {text}");

    public static void Problem(string text) => Write(ConsoleColor.Red, text);

    public static void Table(IReadOnlyList<string> headers, IEnumerable<IReadOnlyList<string>> rows)
    {
        var data = rows.ToList();

        if (data.Count == 0)
        {
            Note("(no rows)");

            return;
        }

        var widths = headers
            .Select((header, column) => Math.Max(header.Length, data.Max(row => row[column].Length)))
            .ToArray();

        Write(ConsoleColor.DarkGray, "    " + Row(headers, widths));
        Write(ConsoleColor.DarkGray, "    " + string.Join("  ", widths.Select(width => new string('-', width))));

        foreach (var row in data)
        {
            Console.WriteLine("    " + Row(row, widths));
        }
    }

    private static string Row(IReadOnlyList<string> cells, IReadOnlyList<int> widths) =>
        string.Join("  ", cells.Select((cell, column) => cell.PadRight(widths[column]))).TrimEnd();

    private static void Write(ConsoleColor color, string text)
    {
        var previous = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ForegroundColor = previous;
    }
}
