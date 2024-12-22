namespace dcsInputBackup;

internal class Backup
{
    public void Execute(string inputSourceRoot, string destination)
    {
        var sourceRoot = DcsLikeDirectory.LooksCredible(inputSourceRoot);
        Directory.CreateDirectory(destination);

        foreach (var aircraftType in new DirectoryInfo(sourceRoot).GetDirectories())
        {
            var stickBindings = Path.Combine(aircraftType.FullName, "joystick");
            if (!Directory.Exists(stickBindings)) continue;
            foreach (var inputFile in Directory.EnumerateFiles(stickBindings,
                         "*.diff.lua"))
            {
                var file = new FileInfo(inputFile);
                if (".diff.lua".Equals(file.Name)) continue;

                var root = EnsureExists(destination, aircraftType.Name);

                var target = Path.Combine(root.FullName, new FileInfo(inputFile).Name);
                File.Copy(inputFile, target);
            }
        }
    }
    
    private static DirectoryInfo EnsureExists(string where, string name)
    {
        var root = Path.Combine(where, name, "joystick");
        return new DirectoryInfo(root).Exists
            ? new DirectoryInfo(root)
            : new DirectoryInfo(Path.Combine(where, name)).CreateSubdirectory("joystick");
    }
}