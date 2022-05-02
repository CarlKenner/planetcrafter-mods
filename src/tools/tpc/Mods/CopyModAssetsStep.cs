namespace Doublestop.Tpc.Mods;

public sealed class CopyModAssetsStep : InstallStep
{
    #region Fields

    const string AssetSourceDirName = "assets";


    #endregion

    #region Public Methods

    public override void OnInstall(InstallPackage package, string pluginsDirectory)
    {
        var assemblyDir = Path.GetDirectoryName(package.SourceAssemblyPath);
        var assetSourceDir = string.IsNullOrWhiteSpace(assemblyDir)
            ? AssetSourceDirName
            : Path.Combine(assemblyDir, AssetSourceDirName);
        if (!Directory.Exists(assetSourceDir))
            // If no assets, return early.
            return;

        // target asset dir layout is plugins/<assembly name>/asset1,asset2...
        var assetTargetDir = Path.Combine(pluginsDirectory, package.TargetAssetsSubDirName);
        CopyDirectory(assetSourceDir, assetTargetDir, true);
    }

    #endregion

    #region Private Methods

    static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
    {
        var dir = new DirectoryInfo(sourceDir);
        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        var dirs = dir.GetDirectories();
        var files = dir.GetFiles();
        if (!dirs.Any() && !files.Any())
            return;

        Directory.CreateDirectory(destinationDir);
        foreach (var file in files)
            file.CopyTo(Path.Combine(destinationDir, file.Name));

        if (!recursive)
            return;

        foreach (var subDir in dirs)
        {
            var newDestinationDir = Path.Combine(destinationDir, subDir.Name);
            CopyDirectory(subDir.FullName, newDestinationDir, true);
        }
    }

    #endregion
}