namespace dcsInputBackup;

public readonly struct InputConfig
{
    public InputConfig(FileInfo file)
    {
        var inputName = file.Name;
        var openCurlyIdx = inputName.IndexOf('{');
        var closeCurlyIdx = inputName.IndexOf('}');

        if (openCurlyIdx > 0 && closeCurlyIdx > 0)
        {
            Name = inputName.Substring(0, openCurlyIdx).Trim();
            var id = inputName.Substring(openCurlyIdx + 1, closeCurlyIdx - openCurlyIdx - 1);
            Guid = new Guid(id);
        }
        else
        {
            Name = inputName.Substring(0, inputName.IndexOf(".diff.lua"));
            Guid = null;
        }
    }

    public readonly String Name;
    public readonly Guid? Guid;

    public string WithNewGuid(Guid instanceGuid)
    {
        return $"{Name} {{{instanceGuid}}}.diff.lua";
    }
}