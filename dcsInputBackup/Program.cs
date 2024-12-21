namespace dcsInputBackup;

internal static class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            PrintHelp();
            return;
        }

        var controllers = new GameControllers();
        switch (args[0].ToLower())
        {
            case "list-devices":
                foreach (var keyValuePair in controllers.Devices())
                {
                    Console.WriteLine($"{keyValuePair.Key} {keyValuePair.Value.InstanceGuid}");
                }

                return;
            case "help":
                PrintHelp();
                return;
            case "update-ids":
                new UpdateIds(controllers).Execute(args.Skip(1).ToArray());
                break;
            case "backup":
                break;
            case "restore":
                new Restore(args.Skip(1).ToArray(), controllers).Execute();
                break;
        }
    }

    private static void PrintHelp()
    {
        Console.WriteLine("Usage: DcsConfigBackup.exe <command> <options>");
        Console.WriteLine("Commands:  list-devices");
        Console.WriteLine("           restore <source> <destination>");
        Console.WriteLine("           backup <source> <destination>");
        Console.WriteLine("           update-ids <destination>");
    }
}