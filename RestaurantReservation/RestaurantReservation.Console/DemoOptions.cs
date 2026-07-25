namespace RestaurantReservation;

public sealed record DemoOptions(IReadOnlyList<string> Sections, bool ApplyMigrations, bool ShowSql, bool HelpRequested, IReadOnlyList<string> UnknownFlags)
{
    public static DemoOptions Parse(string[] args)
    {
        var sections = new List<string>();
        var unknownFlags = new List<string>();
        var applyMigrations = false;
        var showSql = false;
        var helpRequested = false;

        foreach (var arg in args)
        {
            switch (arg.ToLowerInvariant())
            {
                case "--migrate":
                    applyMigrations = true;
                    break;
                case "--sql":
                    showSql = true;
                    break;
                case "--help" or "-h":
                    helpRequested = true;
                    break;
                default:
                    if (arg.StartsWith('-'))
                    {
                        unknownFlags.Add(arg);
                    }
                    else
                    {
                        sections.Add(arg.ToLowerInvariant());
                    }

                    break;
            }
        }

        return new DemoOptions(sections, applyMigrations, showSql, helpRequested, unknownFlags);
    }
}
