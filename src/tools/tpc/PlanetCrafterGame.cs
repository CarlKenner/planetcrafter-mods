using Doublestop.Tpc.Mods;

namespace Doublestop.Tpc;

internal sealed class PlanetCrafterGame
{
    #region Fields

    public const int AppId = 1284190;
    public const string AppIdStr = "1284190";
    public const string GameDirectoryName = "The Planet Crafter";
    public const string BepInExDirectoryName = "BepInEx";
    public const string PluginsDirectoryName = "plugins";

    #endregion

    #region Constructors

    public PlanetCrafterGame(string baseDirectory)
    {
        if (string.IsNullOrWhiteSpace(baseDirectory))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(baseDirectory));

        BaseDirectory = baseDirectory;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Full path to the game installation on the local system.
    /// </summary>
    public string BaseDirectory { get; }

    /// <summary>
    /// Full path to the plugins folder.
    /// </summary>
    public string PluginsDirectory => Path.Combine(
        BaseDirectory, 
        BepInExDirectoryName, 
        PluginsDirectoryName);

    #endregion

    #region Public Methods

    public void InstallMod(InstallPackage package) => 
        new Installer(PluginsDirectory).Install(package);

    public static PlanetCrafterGame? TryLocate()
    {
        var gameDir = new Steam().GetGameDirectory();
        return Directory.Exists(gameDir) ? new PlanetCrafterGame(gameDir) : null;
    }

    #endregion
}