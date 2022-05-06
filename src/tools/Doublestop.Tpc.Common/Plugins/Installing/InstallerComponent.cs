namespace Doublestop.Tpc.Plugins.Installing;

public abstract class InstallerComponent
{
    #region Public Methods

    public abstract ValueTask InstallAsync(PluginPackage package, CancellationToken cancel);

    public abstract ValueTask RemoveAsync(PluginAssembly pluginAssembly, CancellationToken cancel);

    #endregion
}