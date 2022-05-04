using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Doublestop.Tpc.Steam;

internal sealed class SteamHelper
{
    #region Fields

    const string SteamDirName = "Steam";
    internal const string SteamAppsDirName = "steamapps";
    const string CommonDirName = "common";
    const string NonWindowsSteamDirDefault = "/usr/bin/steam";

    #endregion

    #region Constructors

    public SteamHelper(string? steamDirectory = null)
    {
        if (string.IsNullOrWhiteSpace(steamDirectory))
            steamDirectory = AutoDetermineSteamDirectory();

        if (string.IsNullOrWhiteSpace(steamDirectory))
            throw new ArgumentException("A Steam directory wasn't provided, and I couldn't determine one automatically.", nameof(steamDirectory));

        SteamDirectory = new DirectoryInfo(steamDirectory);
        Library = SteamLibraryHelper.CreateDefault(this);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Directory where the steam executable is located. This is also the location of the
    /// root catalog files containing pointers to game installations such as The Planet Crafter.
    /// </summary>
    public DirectoryInfo SteamDirectory { get; }

    /// <summary>
    /// Provides access to the local system's Steam library.
    /// </summary>
    public SteamLibraryHelper Library { get; }

    /// <summary>
    /// Subdirectory where games are installed, eg <c>steamapps/common</c>.
    /// </summary>
    internal static string CommonRelativePath => Path.Combine(SteamAppsDirName, CommonDirName);

    #endregion

    #region Public Methods

    internal static string AutoDetermineSteamDirectory() =>
        GetSteamDirectoryFromRegistry() ??
        GetSteamDirectoryFromProgramFiles() ??
        NotEntirelyHardCodedDefault();

    internal static string? GetSteamDirectoryFromRegistry()
    {
        // Can't really stop someone from building/running this on another platform.
        // Might as well shoot for grace over implosion.
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return null;

        const string steamKeyPath = @"Software\Valve\Steam";
        const string pathValueName = "SteamPath";
        const string exeValueName = "SteamExe";
        using var steamKey = Registry.CurrentUser.OpenSubKey(steamKeyPath);
        if (steamKey is null)
            return null;

        // try the SteamPath key first.
        var steamDir = Convert.ToString(steamKey.GetValue(pathValueName, null));
        if (!string.IsNullOrWhiteSpace(steamDir))
            return Path.GetFullPath(steamDir);

        // try the SteamExe key and, if found, strip away the filename and return the directory.
        var steamExe = Convert.ToString(steamKey.GetValue(exeValueName, null));
        if (!string.IsNullOrWhiteSpace(steamExe) &&
            Path.GetDirectoryName(steamExe) is { } dirFromExe)
            return Path.GetFullPath(dirFromExe);

        // bunk.
        return null;
    }

    internal static string? GetSteamDirectoryFromProgramFiles()
    {
        // Can't really stop someone from building/running this on another platform.
        // Might as well shoot for grace over implosion.
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return null;

        // 32-bit program files
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
            SteamDirName);
        if (Directory.Exists(dir))
            return dir;

        // 64-bit program files
        dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            SteamDirName);
        if (Directory.Exists(dir))
            return dir;

        return null;
    }

    internal static string NotEntirelyHardCodedDefault()
    {
        // linux, mac, etc.
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return NonWindowsSteamDirDefault;

        // windows. assumes program files x86.
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
            SteamDirName);
    }

    #endregion
}