using Doublestop.Tpc.Plugins;

namespace Doublestop.Tpc.Game;

public sealed class BepInExHelper
{
    #region Fields

    public const string CoreDirectoryBaseName = "core";
    public const string PluginsDirectoryBaseName = "plugins";

    #endregion

    #region Constructors

    public BepInExHelper(string coreDirectory, string pluginsDirectory)
    {
        if (string.IsNullOrWhiteSpace(coreDirectory))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(coreDirectory));
        if (string.IsNullOrWhiteSpace(pluginsDirectory))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(pluginsDirectory));

        CoreDirectory = new DirectoryInfo(coreDirectory);
        PluginsDirectory = new DirectoryInfo(pluginsDirectory);
    }

    #endregion

    #region Properties

    public DirectoryInfo CoreDirectory { get; }
    public DirectoryInfo PluginsDirectory { get; }

    /// <summary>
    /// Enumerates <c>dll</c> files in the <see cref="CoreDirectory"/>.
    /// </summary>
    public IEnumerable<string> CoreDlls =>
        CoreDirectory
            .EnumerateFiles("*.dll", SearchOption.TopDirectoryOnly)
            .Select(f => f.FullName);

    /// <summary>
    /// Enumerates <c>dll</c> files in the <see cref="PluginsDirectory"/>.
    /// </summary>
    public IEnumerable<string> PluginDlls =>
        PluginsDirectory
            .EnumerateFiles("*.dll", SearchOption.TopDirectoryOnly)
            .Select(f => f.FullName);

    #endregion

    #region Public Methods

    public static BepInExHelper Create(string gameDirectory)
    {
        var bepinexRoot = Path.Combine(gameDirectory, ThePlanetCrafter.BepInExDirectoryName);
        return new BepInExHelper(
            Path.Combine(bepinexRoot, CoreDirectoryBaseName),
            Path.Combine(bepinexRoot, PluginsDirectoryBaseName));
    }

    public IEnumerable<PluginAssembly> EnumerateAssemblies()
    {
        foreach (var file in PluginDlls)
            if (GetAssemblyByPath(file) is { } assembly)
                yield return assembly;
    }

    /// <summary>
    /// Loads a plugin by file name, with or without the <c>.dll</c> extension.
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <remarks>
    /// If any path information is present in <see cref="assemblyName"/>, it will be ignored.
    /// </remarks>
    public PluginAssembly? GetAssembly(string assemblyName)
    {
        if (assemblyName == null) throw new ArgumentNullException(nameof(assemblyName));

        // strip path info from the assembly name, just in case
        assemblyName = Path.GetFileName(assemblyName);
        return GetAssemblyByPath(Path.Combine(PluginsDirectory.FullName, assemblyName)) ??
               GetAssemblyByPath(Path.Combine(PluginsDirectory.FullName, $"{assemblyName}.dll"));
    }

    #endregion

    #region Private Methods

    PluginAssembly? GetAssemblyByPath(string assemblyPath) =>
        File.Exists(assemblyPath) 
            ? new PluginAssembly(assemblyPath, CoreDlls) 
            : null;

    #endregion
}