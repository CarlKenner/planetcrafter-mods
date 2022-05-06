using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using ValveKeyValue;

namespace Doublestop.Tpc.Steam;

/// <summary>
/// Provides read access to the Steam library database on the local system.
/// </summary>
[DebuggerDisplay("{_libraryFoldersVdfFile}")]
internal sealed class SteamLibraryHelper
{
    #region Fields

    internal const string LibraryFoldersFileName = "libraryfolders.vdf";


    readonly FileInfo _libraryFoldersVdfFile;

    #endregion

    #region Constructors

    public SteamLibraryHelper(FileInfo libraryFoldersVdfFile)
    {
        _libraryFoldersVdfFile = libraryFoldersVdfFile ?? throw new ArgumentNullException(nameof(libraryFoldersVdfFile));
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// I should be documented, but I'm not.
    /// </summary>
    /// <param name="steamDirectory"></param>
    /// <returns></returns>
    public static SteamLibraryHelper CreateDefault(string steamDirectory) =>
        new(new FileInfo(Path.Combine(
            steamDirectory,
            SteamHelper.SteamAppsDirName,
            LibraryFoldersFileName)));

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
        var serializer = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);

        try
        {
            var libraryFolders = Deserialize(_libraryFoldersVdfFile.FullName);
            foreach (var folder in libraryFolders.Children)
            {
                // Look for this app id in the folder["apps"] collection.
                if (!ContainsAppId(folder, out var folderDirectory)) 
                    continue;

                // Read the "installdir" property from the app's manifest file.
                if (!ReadAppManifestInstallDir(folderDirectory, out var installDir))
                    // file not found, or installdir not set / bad value. keep looking.
                    continue;

                // installDir is just a directory name, so we need to
                // combine it with "folder path/common" to get the full path.
                // eg folder["path"]/common/<installDir>
                var gameDir = Path.Combine(folderDirectory, SteamHelper.CommonRelativePath, installDir);
                if (!Directory.Exists(gameDir))
                    // well we got this far, but the directory wasn't found.
                    // keep looking. maybe it was moved and Steam failed to update the library files. who knows.
                    continue;

                return new DirectoryInfo(gameDir);
            }

            // looked through all the libraries, couldn't find this app id. 
            return null;
        }
        catch (FileNotFoundException ex)
        {
            throw new FileNotFoundException(
                $"Steam Library file [{_libraryFoldersVdfFile.Name}] was not found.",
                ex);
        }

        #region Local Helpers

        KVObject Deserialize(string file)
        {
            using var stream = File.Open(file, FileMode.Open, FileAccess.Read);
            return serializer.Deserialize(stream);
        }

        // Attempts to open and read "installdir" from the app id's manifest in the provided directory.
        bool ReadAppManifestInstallDir(string folderDirectory, [NotNullWhen(true)] out string? installDirName)
        {
            var appManifestFile = Path.Combine(folderDirectory, SteamHelper.SteamAppsDirName, $"appmanifest_{appId}.acf");
            if (!File.Exists(appManifestFile))
            {
                installDirName = null;
                return false;
            }
            // If found, the corresponding value marks the name of the directory where the game was installed
            var appManifest = Deserialize(appManifestFile);
            installDirName = appManifest["installdir"].ToString(CultureInfo.CurrentCulture);
            return !string.IsNullOrWhiteSpace(installDirName);
        }

        // Returns true if folder[apps] contains the app id we're looking for.
        bool ContainsAppId(KVObject folder, [NotNullWhen(true)] out string? libraryFolderPath)
        {
            libraryFolderPath = null;
            var containsApp = folder.Children.Any(c => c.Name == "apps" &&
                                                       c.Children.Any(o => IsSameAppId(o, appId)));

            if (containsApp)
                libraryFolderPath = folder["path"]?.ToString(CultureInfo.CurrentCulture);

            return !string.IsNullOrWhiteSpace(libraryFolderPath);

            static bool IsSameAppId(KVObject obj, int appId) =>
                int.TryParse(obj.Name, out var n) && n == appId;
        }

        #endregion
    }

    #endregion
}