namespace Thangs.Tpc.Mods;

internal static class InstallUtil
{
    #region Public Methods

    internal static string FindFirstAndOnlyAssemblySourcePath(string directory)
    {
        var files = new DirectoryInfo(directory).EnumerateFiles("*.dll", SearchOption.TopDirectoryOnly);
        FileInfo? assemblyFile = null;
        foreach (var file in files)
        {
            if (assemblyFile != null)
                throw new TooManyModAssembiesFoundException();

            assemblyFile = file;
        }
        if (assemblyFile is null)
            throw new ModAssemblyNotFoundException();

        return assemblyFile.FullName;
    }

    #endregion
}