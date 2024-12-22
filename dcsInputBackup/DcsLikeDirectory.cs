namespace dcsInputBackup;

internal static class DcsLikeDirectory
{
    internal static string LooksCredible(string inputSourceRoot)
    {
        if (Directory.Exists(inputSourceRoot))
        {
            var rootConfigInput = Path.Combine(inputSourceRoot, "Config", "input");
            if (Directory.Exists(rootConfigInput))
            {
                return rootConfigInput;
            }

            var rootInput = Path.Combine(inputSourceRoot, "input");
            if (Directory.Exists(rootInput))
            {
                return rootInput;
            }
        }

        Console.WriteLine("Input source does not look like DCS input config directory.");
        throw new Exception("Invalid arguments");
    }
}