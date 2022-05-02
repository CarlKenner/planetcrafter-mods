namespace Doublestop.Tpc;

internal sealed class Steam
{
    #region Fields

    readonly string _steamInstallDir;

    #endregion

    #region Constructors

    public Steam(string? steamInstallDir = null)
    {
        _steamInstallDir = string.IsNullOrWhiteSpace(steamInstallDir)
            // todo: automatically determine steam installation location
            // maybe through the registry.
            // hard coding to my local atm.
            ? @"D:\Games\Steam"
            : steamInstallDir;
    }

    #endregion

    #region Properties

    static string AppsSubPath => Path.Combine("steamapps", "common");

    #endregion

    #region Public Methods

    public string GetGameDirectory()
    {
        // todo: look up TPC's install dir from the local steam libraries
        // libraryfolders.vdf: app id -> steam library folder path
        // library path/.../appmanifest_1284190.acf: contains game's "installdir" directory name under the lib's steamapps/common.

        // For now, assume the game is under <_steamInstallDir>/<AppsSubPath>/The Planet Crafter
        return Path.GetFullPath(
            Path.Combine(
                _steamInstallDir,
                AppsSubPath,
                PlanetCrafterGame.GameDirectoryName));

    }

    #endregion
}