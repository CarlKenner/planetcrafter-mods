namespace Doublestop.Tpc.Mods;

internal static class InstallUtil
{
    #region Public Methods

    /// <summary>
    /// Searches a directory for a mod assembly (.dll). If more than one .dll file is found, an error is thrown.
    /// If less than one .dll file is found, an error is thrown. It's really important for this directory to contain exactly one .dll.
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    /// <exception cref="TooManyModAssembliesFoundException"></exception>
    /// <exception cref="ModAssemblyNotFoundException"></exception>
    internal static string FindOneAndOnlyAssembly(string directory)
    {
        var files = new DirectoryInfo(directory).EnumerateFiles("*.dll", SearchOption.TopDirectoryOnly);
        FileInfo? assemblyFile = null;
        foreach (var file in files)
        {
            if (assemblyFile != null)
                // one at a time, please.
                throw new TooManyModAssembliesFoundException();

            assemblyFile = file;
        }
        if (assemblyFile is null)
            // not zero at a time, please.
            throw new ModAssemblyNotFoundException();

        return assemblyFile.FullName;
    }

    #endregion
}