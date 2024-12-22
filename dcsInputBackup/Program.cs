namespace dcsInputBackup;

internal static class Program
{
    private static readonly GameControllers Controllers = new();

    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            PrintHelp();
            return;
        }

        var command = args[0].ToLower();
        var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var dcsInUserSavedGames = Path.Combine(userProfile, "Saved Games", "DCS.Openbeta");
        var remainingArgs = args.Skip(1).ToArray();

        _ = (command, remainingArgs) switch
        {
            ("help", _) => ExecuteAction(PrintHelp),
            ("list-devices", _) => ExecuteAction(ListDevices),
            ("update-ids", [var destination]) => ExecuteAction(() =>
                new UpdateIds(Controllers).Execute([destination])),
            ("backup", [var destination]) => ExecuteAction(() => new Backup().Execute(dcsInUserSavedGames, destination)),
            ("restore", [var source]) => ExecuteAction(() =>
                new Restore(Controllers).Execute(source, dcsInUserSavedGames)),
            ("restore", [var source, var destination]) => ExecuteAction(() =>
                new Restore(Controllers).Execute(source, destination)),
            _ => ExecuteAction(PrintHelp)
        };
    }

    private static void ListDevices()
    {
        foreach (var keyValuePair in Controllers.Devices())
        {
            Console.WriteLine($"{keyValuePair.Key} {keyValuePair.Value.InstanceGuid}");
        }
    }

    private static void PrintHelp()
    {
        Console.WriteLine("Usage: DcsConfigBackup.exe <command> <options>");
        Console.WriteLine("Commands:  list-devices");
        Console.WriteLine("           restore <source> (assume destination in user's Saved Games)");
        Console.WriteLine("           restore <source> <destination>");
        Console.WriteLine("           backup <destination> (assume source in user's Saved Games)");
        Console.WriteLine("           backup <destination> <source>");
        Console.WriteLine("           update-ids (assume destination in user's Saved Games)");
        Console.WriteLine("           update-ids <destination>");
    }

    private static object? ExecuteAction(Action action)
    {
        action();
        return null;
    }
}