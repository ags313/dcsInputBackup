namespace dcsInputBackup;

internal class Restore
{
    private readonly GameControllers _gameControllers;
    private readonly string _sourceRoot;
    private readonly string _destinationRoot;

    public Restore(string[] args, GameControllers gameControllers)
    {
        _gameControllers = gameControllers;
        if (args.Length < 1)
        {
            WriteHelp();
            throw new Exception("Invalid arguments");
        }

        var backupRoot = args[0];
        if (Directory.Exists(backupRoot))
        {
            var rootInput = Path.Combine(backupRoot, "input");
            var rootConfigInput = Path.Combine(backupRoot, "Config", "input");
            if (Directory.Exists(rootConfigInput))
            {
                _sourceRoot = rootConfigInput;
            }
            else if (Directory.Exists(rootInput))
            {
                _sourceRoot = rootInput;
            }
        }
        else
        {
            Console.WriteLine("Backup directory does not exist.");
            throw new Exception("Invalid arguments");
        }

        var destinationRoot = args.Length > 1
            ? args[1]
            : Path.Combine(Environment.SpecialFolder.UserProfile.ToString(), "Saved Games", "DCS.Openbeta");

        if (Directory.Exists(destinationRoot))
        {
            var destinationInput = Path.Combine(destinationRoot, "input");
            var destinationConfigInput = Path.Combine(destinationRoot, "Config", "input");
            if (Directory.Exists(destinationConfigInput))
            {
                _destinationRoot = destinationConfigInput;
            }
            else if (Directory.Exists(destinationInput))
            {
                _destinationRoot = destinationInput;
            }
        }
        else
        {
            Console.WriteLine($"Destination directory ${destinationRoot} does not exist.");
            throw new Exception("Invalid arguments");
        }
    }

    public void Execute()
    {
        var knownDevices = _gameControllers.Devices();
        foreach (var airplaneRoot in Directory.EnumerateDirectories(_sourceRoot))
        {
            var airplaneRootDirectory = new DirectoryInfo(airplaneRoot);
            var planeDestination = Path.Combine(_destinationRoot, airplaneRootDirectory.Name, "joystick");
            Directory.CreateDirectory(planeDestination);
            foreach (var stickConfig in Directory.EnumerateFiles(Path.Combine(airplaneRoot, "joystick"),
                         "*.diff.lua"))
            {
                var backupFile = new InputConfig(new FileInfo(stickConfig));
                if (knownDevices.TryGetValue(backupFile.Name, out var deviceInOs))
                {
                    var newName = backupFile.WithNewGuid(deviceInOs.InstanceGuid);
                    var pathToCopyTo = Path.Combine(planeDestination, Path.GetFileName(newName));
                    if (File.Exists(pathToCopyTo))
                    {
                        Console.WriteLine($"Skipping {backupFile.Name} as it already exists.");
                    }
                    else
                    {
                        Console.WriteLine($"Restoring, update GUID: {backupFile.Name}");
                        File.Copy(stickConfig, pathToCopyTo, false);
                    }
                }
                else
                {
                    Console.WriteLine($"Restoring, old GUID: {backupFile.Name}");
                }
            }
        }
    }

    private static void WriteHelp()
    {
        Console.WriteLine("Restore requires 2 arguments: <source> <destination>");
        Console.WriteLine("Source is the directory containing the config backup.");
        Console.WriteLine(@"Destination is usually %USERPROFILE%\Saved Games\DCS.Openbeta\");
    }
}