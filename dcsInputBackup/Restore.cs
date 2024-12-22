namespace dcsInputBackup;

internal class Restore(GameControllers gameControllers)
{
    public void Execute(string inputSourceRoot, string inputDestinationRoot)
    {
        var sourceRoot = DcsLikeDirectory.LooksCredible(inputSourceRoot);
        var destinationRoot = DcsLikeDirectory.LooksCredible(inputDestinationRoot);
        
        var knownDevices = gameControllers.Devices();
        foreach (var airplaneRoot in Directory.EnumerateDirectories(sourceRoot))
        {
            var airplaneRootDirectory = new DirectoryInfo(airplaneRoot);
            var planeDestination = Path.Combine(destinationRoot, airplaneRootDirectory.Name, "joystick");
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
}