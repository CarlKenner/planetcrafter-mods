using Doublestop.Tpc.Plugins;

namespace Doublestop.Tpc;

public sealed class ThePlanetCrafter
{
    #region Fields

    public const int AppId = 1284190;
    public const string AppIdStr = "1284190";
    public const string GameDirectoryName = "The Planet Crafter";
    public const string BepInExDirectoryName = "BepInEx";

    #endregion

    #region Constructors

    public ThePlanetCrafter(string directory, BepInExHelper? bepInExInfo = null, LocalPlugins? plugins = null)
    {
        Directory = new DirectoryInfo(directory);
        BepInEx = bepInExInfo ?? BepInExHelper.CreateDefault(Directory.FullName);
        Plugins = plugins ?? new LocalPlugins(this);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Full path to the game installation on the local system.
    /// </summary>
    public DirectoryInfo Directory { get; }

    /// <summary>
    /// Plugin management.
    /// </summary>
    public LocalPlugins Plugins { get; }

    /// <summary>
    /// Contains information about the BepInEx configuration.
    /// </summary>
    public BepInExHelper BepInEx { get; }

    #endregion
}