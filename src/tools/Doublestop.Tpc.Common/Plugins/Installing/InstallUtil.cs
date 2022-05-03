namespace Doublestop.Tpc.Plugins.Installing;

internal static class InstallUtil
{
    #region Public Methods

    /// <summary>
    /// Searches a directory for a plugin assembly (.dll). If more than one .dll file is found, an error is thrown.
    /// If less than one .dll file is found, an error is thrown. It's really important for this directory to contain exactly one .dll.
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    /// <exception cref="TooManyPluginAssembliesFoundException"></exception>
    /// <exception cref="PluginAssemblyNotFoundException"></exception>
    internal static string FindOneAndOnlyAssembly(string directory)
    {
        var files = new DirectoryInfo(directory).EnumerateFiles("*.dll", SearchOption.TopDirectoryOnly);
        FileInfo? assemblyFile = null;
        foreach (var file in files)
        {
            if (assemblyFile != null)
                throw new TooManyPluginAssembliesFoundException();

            assemblyFile = file;
        }
        if (assemblyFile is null)
            throw new PluginAssemblyNotFoundException();

        return assemblyFile.FullName;
    }

    #endregion
}