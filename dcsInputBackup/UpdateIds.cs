namespace dcsInputBackup;

internal class UpdateIds(GameControllers controllers)
{
    public void Execute(string[] toArray)
    {
        var suspectedRoot = toArray[0];
        var knownDevices = controllers.Devices();

        var rootDir = "";
        if (Directory.Exists(Path.Combine(suspectedRoot, "config", "input")))
        {
            rootDir = Path.Combine(suspectedRoot, "config", "input");
        }
        else if (Directory.Exists(Path.Combine(suspectedRoot, "input")))
        {
            rootDir = Path.Combine(suspectedRoot, "input");
        }

        foreach (var airplaneRoot in Directory.EnumerateDirectories(rootDir))
        {
            Console.WriteLine($"Updating IDs for {airplaneRoot}");
            foreach (var stickConfig in Directory.EnumerateFiles(Path.Combine(airplaneRoot, "joystick"),
                         "*.diff.lua"))
            {
                var fileInfo = new FileInfo(stickConfig);
                var backupFile = new InputConfig(fileInfo);
                if (!knownDevices.TryGetValue(backupFile.Name, out var deviceInOs)) continue;
                if (deviceInOs.InstanceGuid.Equals(backupFile.Guid)) continue;
                    
                var newName = backupFile.WithNewGuid(deviceInOs.InstanceGuid);
                var target = Path.Combine(fileInfo.Directory.FullName, newName);
                if (File.Exists(target))
                {
                    Console.WriteLine($"Skipping {fileInfo.Name}, already exists.");
                }
                else
                {
                    Console.WriteLine($"Renaming {fileInfo.Name} to {target}");
                    File.Move(stickConfig, target);
                }
            }
        }
    }
}