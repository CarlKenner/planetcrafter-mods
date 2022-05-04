using Doublestop.Tpc.Internal;

namespace Doublestop.Tpc.Plugins.Installing.Components;

internal sealed class AssetsInstallerComponent : InstallerComponent
{
    #region Fields

    const string AssetSourceDirName = "assets";

    readonly string _pluginsDirectory;

    #endregion

    #region Constructors

    public AssetsInstallerComponent(string pluginsDirectory)
    {
        if (string.IsNullOrWhiteSpace(pluginsDirectory))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(pluginsDirectory));
        _pluginsDirectory = pluginsDirectory;
    }

    #endregion

    #region Public Methods

    public override async ValueTask InstallAsync(PluginPackage package, CancellationToken cancel)
    {
        await Task.Run(() =>
        {
            var assemblyDir = Path.GetDirectoryName(package.SourceAssemblyPath);
            var assetSourceDir = string.IsNullOrWhiteSpace(assemblyDir)
                ? AssetSourceDirName
                : Path.Combine(assemblyDir, AssetSourceDirName);
            if (!Directory.Exists(assetSourceDir))
                // If no assets, return early.
                return;

            // target asset dir layout is plugins/<assembly name>/asset1,asset2...
            var assetTargetDir = Path.Combine(_pluginsDirectory, Path.GetFileNameWithoutExtension(package.TargetAssemblyFileName));
            FileUtil.CopyDirectoryTree(assetSourceDir, assetTargetDir, cancel);
        }, cancel);
    }

    public override async ValueTask RemoveAsync(PluginFile pluginFile, CancellationToken cancel)
    {
        await Task.Run(() =>
        {
            var pluginDir = pluginFile.Directory ??
                            Path.GetPathRoot(Environment.CurrentDirectory) ??
                            Path.GetFullPath("/");
            var assetsDir = Path.Combine(pluginDir, pluginFile.NameWithoutExtension);
            if (Directory.Exists(assetsDir))
                Directory.Delete(assetsDir, true);
        }, cancel);
    }

    #endregion
}