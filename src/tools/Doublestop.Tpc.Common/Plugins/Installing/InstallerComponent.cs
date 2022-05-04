namespace Doublestop.Tpc.Plugins.Installing;

internal abstract class InstallerComponent
{
    #region Public Methods

    public abstract ValueTask InstallAsync(PluginPackage package, CancellationToken cancel);

    public abstract ValueTask RemoveAsync(PluginFile pluginFile, CancellationToken cancel);

    #endregion
}