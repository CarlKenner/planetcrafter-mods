using System.Diagnostics;
using Doublestop.Tpc.Game;
using Doublestop.Tpc.Plugins;
using Doublestop.Tpc.Plugins.Installing;
using Doublestop.Tpc.Steam;

namespace Doublestop.Tpc;

[DebuggerDisplay("{GameDirectory}")]
public sealed class ThePlanetCrafter
{
    #region Fields

    public const int AppId = 1284190;
    public const string BepInExDirectoryName = "BepInEx";
    public const string ExeName = "Planet Crafter.exe";

    readonly PluginInstaller _installer;

    #endregion

    #region Constructors

    public ThePlanetCrafter(string? gameDirectory = null, BepInExHelper? bepInEx = null, LocalPlugins? plugins = null)
    {
        if (string.IsNullOrWhiteSpace(gameDirectory))
            gameDirectory = AutoDetectGameDirectory();

        GameDirectory = new DirectoryInfo(gameDirectory);
        GameExecutable = new FileInfo(Path.Combine(GameDirectory.FullName, ExeName));

        bepInEx ??= BepInExHelper.Create(GameDirectory.FullName);
        Plugins = plugins ?? new LocalPlugins(bepInEx);
        _installer = new PluginInstaller(bepInEx.PluginsDirectory.FullName);
    }

    static string InitializeGameDir(string? gameDir) => string.IsNullOrWhiteSpace(gameDir)
        ? AutoDetectGameDirectory()
        : gameDir;

    #endregion

    #region Properties

    /// <summary>
    /// Full path to the game installation on the local system.
    /// </summary>
    public DirectoryInfo GameDirectory { get; }

    /// <summary>
    /// The game's executable file.
    /// </summary>
    public FileInfo GameExecutable { get; }

    /// <summary>
    /// Plugin management.
    /// </summary>
    public LocalPlugins Plugins { get; }

    #endregion

    public async ValueTask<PluginAssembly> InstallPluginAssembly(PluginPackage plugin, CancellationToken cancel)
    {
        await _installer.InstallAsync(plugin, cancel);
        return Plugins.GetAssembly(plugin.TargetAssemblyFileName) ??
               throw new PluginAssemblyNotFoundException($"{plugin.TargetAssemblyFileName} was not found after installation.");
    }

    public async ValueTask RemovePluginAssembly(PluginAssembly assembly, CancellationToken cancel)
    {
        await _installer.RemoveAsync(assembly, cancel);
    }

    public async ValueTask RemovePluginAssembly(string assemblyName, CancellationToken cancel)
    {
        var assembly = Plugins.GetAssembly(assemblyName);
        if (assembly is null)
            throw new FileNotFoundException();

        await RemovePluginAssembly(assembly, cancel);
    }

    #region Private Methods

    static string AutoDetectGameDirectory() =>
        new SteamHelper().Library.GetGameDirectory(AppId)?.FullName ??
        throw new Exception("Game directory not specified, and it could not be discovered automatically.");

    #endregion
}