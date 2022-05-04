using System.Diagnostics;
using System.Globalization;
using ValveKeyValue;

namespace Doublestop.Tpc.Steam;

/// <summary>
/// Provides read access to the Steam library database on the local system.
/// </summary>
[DebuggerDisplay("{_libraryFoldersFilePath}")]
internal sealed class SteamLibraryHelper
{
    #region Fields

    internal const string LibraryFoldersFileName = "libraryfolders.vdf";

    readonly string _libraryFoldersFilePath;

    #endregion

    #region Constructors

    public SteamLibraryHelper(string libraryFoldersFilePath)
    {
        if (string.IsNullOrWhiteSpace(libraryFoldersFilePath))
            throw new ArgumentException("Library folders file path cannot be null or whitespace.", nameof(libraryFoldersFilePath));

        _libraryFoldersFilePath = libraryFoldersFilePath;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// I should be documented, but I'm not.
    /// </summary>
    /// <param name="steamDirectory"></param>
    /// <returns></returns>
    public static SteamLibraryHelper CreateDefault(string steamDirectory) =>
        new(Path.Combine(
            steamDirectory,
            SteamHelper.SteamAppsDirName,
            LibraryFoldersFileName));

    /// <summary>
    /// I should be documented, but I'm not.
    /// </summary>
    /// <param name="steam"></param>
    /// <returns></returns>
    public static SteamLibraryHelper CreateDefault(SteamHelper steam) =>
        CreateDefault(steam.SteamDirectory.FullName);

    /// <summary>
    /// I should be documented, but I'm not.
    /// </summary>
    /// <param name="appId"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    public DirectoryInfo? GetGameDirectory(int appId)
    {
        // todo: refactor, at the very least splitting the loop into methods.
        var serializer = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);

        KVObject Deserialize(string file)
        {
            using var stream = File.Open(file, FileMode.Open, FileAccess.Read);
            return serializer.Deserialize(stream);
        }

        try
        {
            var libraryFolders = Deserialize(_libraryFoldersFilePath);
            foreach (var folder in libraryFolders.Children)
            {
                // Look for an "apps" child object.
                var appIds = folder.Children.FirstOrDefault(t => t.Name == "apps");
                if (appIds?.Value.ValueType is not KVValueType.Collection)
                    continue;

                // "apps" is a collection of appid:idk_timestamp_maybe pairs.
                // We're only interested in locating the app id, no interest in the value.
                if (appIds.Children.All(o => !IsSameAppId(o, appId)))
                    // this app id wasn't found in this folder. keep looking in the next folder.
                    continue;

                // Found the app id. Now, need to open the folder referenced by folder["path"]
                // and look for the app manifest: appmanifest_<appId>.acf
                var folderDirectory = folder["path"]?.ToString(CultureInfo.CurrentCulture);
                if (string.IsNullOrWhiteSpace(folderDirectory))
                    // "path" value not set. try another folder.
                    continue;

                var appManifestFile = Path.Combine(folderDirectory, SteamHelper.SteamAppsDirName, $"appmanifest_{appId}.acf");
                if (!File.Exists(appManifestFile))
                    // Not found. Probably won't see this app id again, but keep going anyway. Never know! 
                    continue;

                // Deserialize the manifest and look for the "installdir" property.
                // If found, the corresponding value marks the name of the directory where the game was installed,
                // under this library folder's "steamapps/common" directory.
                // eg, <this library>/steamapps/common/The Planet Crafter
                var appManifest = Deserialize(appManifestFile);
                var installDir = appManifest["installdir"].ToString(CultureInfo.CurrentCulture);
                if (string.IsNullOrWhiteSpace(installDir))
                    // "installdir" value missing from the manifest. keep looking.
                    continue;

                // Since installDir is just a directory name, not a path, we need to
                // build a complete path using all the other stuff we know.
                // <this library folder's path>/common/<installDir>
                var gameDir = Path.Combine(folderDirectory, SteamHelper.CommonRelativePath, installDir);
                if (!Directory.Exists(gameDir))
                    // well we got this far, but the directory wasn't found.
                    // keeeeeeep looking. maybe it was moved and Steam failed to update the library files. who knows.
                    // It could still be out there, waiting for us.
                    // It's getting late. The wind howls.
                    // You can't see into the dark that lies ahead, but you sense something is there. Continue?
                    // > _
                    continue;

                return new DirectoryInfo(gameDir);
            }

            // looked through all the libraries, couldn't find this app id. 
            return null;
        }
        catch (FileNotFoundException ex)
        {
            throw new FileNotFoundException(
                $"Steam Library file [{Path.GetFileName(_libraryFoldersFilePath)}] was not found.",
                ex);
        }

        static bool IsSameAppId(KVObject obj, int appId) =>
            int.TryParse(obj.Name, out var n) && n == appId;
    }

    #endregion
}