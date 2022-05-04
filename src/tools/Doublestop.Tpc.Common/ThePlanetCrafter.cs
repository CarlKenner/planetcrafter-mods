using System.Diagnostics;
using Doublestop.Tpc.Game;
using Doublestop.Tpc.Plugins;
using Doublestop.Tpc.Steam;

namespace Doublestop.Tpc;

[DebuggerDisplay("{GameDirectory}")]
public sealed class ThePlanetCrafter
{
    #region Fields

    public const int AppId = 1284190;
    public const string GameDirectoryName = "The Planet Crafter";
    public const string BepInExDirectoryName = "BepInEx";
    public const string ExeName = "Planet Crafter.exe";
    public const string DataDirName = "Planet Crafter_Data";
    public const string ManagedDirName = "Managed";

    #endregion

    #region Constructors

    public ThePlanetCrafter(string? gameDirectory = null, BepInExHelper? bepInExInfo = null, LocalPlugins? plugins = null)
    {
        if (string.IsNullOrWhiteSpace(gameDirectory))
            gameDirectory = AutoDetectGameDirectory();
        GameDirectory = new DirectoryInfo(gameDirectory);
        BepInEx = bepInExInfo ?? BepInExHelper.CreateDefault(GameDirectory.FullName);
        Plugins = plugins ?? new LocalPlugins(BepInEx);
        ManagedFiles = new ManagedFiles(Path.Combine(
            GameDirectory.FullName,
            DataDirName,
            ManagedDirName));
        GameExecutable = new FileInfo(Path.Combine(GameDirectory.FullName, ExeName));

        static string AutoDetectGameDirectory() =>
            new SteamHelper().Library.GetGameDirectory(AppId)?.FullName ??
            throw new ArgumentException("Game directory not specified, and I could not locate it through Steam.", nameof(plugins));
    }

    #endregion

    #region Properties

    /// <summary>
    /// Full path to the game installation on the local system.
    /// </summary>
    public DirectoryInfo GameDirectory { get; }

    /// <summary>
    /// Plugin management.
    /// </summary>
    public LocalPlugins Plugins { get; }

    /// <summary>
    /// Contains information about the BepInEx configuration.
    /// </summary>
    public BepInExHelper BepInEx { get; }

    /// <summary>
    /// The game's own .NET assemblies, including <c>Assembly-CSharp.dll</c>.
    /// </summary>
    public ManagedFiles ManagedFiles { get; }

    /// <summary>
    /// The game's executable file.
    /// </summary>
    public FileInfo GameExecutable { get; }

    #endregion
}