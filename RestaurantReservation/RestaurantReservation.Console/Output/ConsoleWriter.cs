namespace RestaurantReservation.Output;

public static class ConsoleWriter
{
    private const string Indent = "  ";

    public static void Section(int number, string title)
    {
        Write(ConsoleColor.Cyan, $"===[ {number}. {title} ]===");
        Console.WriteLine();
    }

    public static void Step(string text)
    {
        Console.WriteLine();
        Write(ConsoleColor.White, $"{Indent}> {text}");
    }

    public static void Created(string text) => Marker(ConsoleColor.Green, '+', "created", text);

    public static void Updated(string text) => Marker(ConsoleColor.Yellow, '~', "updated", text);

    public static void Deleted(string text) => Marker(ConsoleColor.Magenta, '-', "deleted", text);

    public static void Value(string label, string value)
    {
        Console.Write($"{Indent}{Indent}{label}: ");
        Write(ConsoleColor.White, value);
    }

    public static void Info(string text) => Write(ConsoleColor.DarkGray, $"{Indent}{Indent}{text}");

    public static void Detail(string text) => Console.WriteLine($"{Indent}{Indent}{text}");

    public static void Warning(string text) => Write(ConsoleColor.Yellow, $"{Indent}{text}");

    public static void Error(string text) => Write(ConsoleColor.Red, $"{Indent}{text}");

    public static void Line(string text = "") => Console.WriteLine(text);

    public static void Table(IReadOnlyList<string> headers, IEnumerable<IReadOnlyList<string>> rows)
    {
        var data = rows.ToList();

        if (data.Count == 0)
        {
            Info("(no rows)");

            return;
        }

        var widths = new int[headers.Count];
        var numeric = new bool[headers.Count];

        for (var column = 0; column < headers.Count; column++)
        {
            widths[column] = Math.Max(headers[column].Length, data.Max(row => row[column].Length));
            numeric[column] = data.All(row => decimal.TryParse(row[column], out _));
        }

        Write(ConsoleColor.DarkGray, Indent + Indent + Format(headers, widths, numeric));
        Write(ConsoleColor.DarkGray, Indent + Indent + string.Join("  ", widths.Select(width => new string('-', width))));

        foreach (var row in data)
        {
            Console.WriteLine(Indent + Indent + Format(row, widths, numeric));
        }
    }

    private static string Format(IReadOnlyList<string> cells, IReadOnlyList<int> widths, IReadOnlyList<bool> numeric)
    {
        var padded = cells.Select((cell, column) => numeric[column]
            ? cell.PadLeft(widths[column])
            : cell.PadRight(widths[column]));

        return string.Join("  ", padded).TrimEnd();
    }

    private static void Marker(ConsoleColor color, char symbol, string verb, string text)
    {
        Console.Write($"{Indent}{Indent}");
        Write(color, $"{symbol} {verb}", newLine: false);
        Console.WriteLine($"  {text}");
    }

    private static void Write(ConsoleColor color, string text, bool newLine = true)
    {
        var previous = Console.ForegroundColor;
        Console.ForegroundColor = color;

        if (newLine)
        {
            Console.WriteLine(text);
        }
        else
        {
            Console.Write(text);
        }

        Console.ForegroundColor = previous;
    }
}
