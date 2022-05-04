using Doublestop.Tpc.Game;
using Doublestop.Tpc.Plugins.Installing.Components;

namespace Doublestop.Tpc.Plugins.Installing;

public sealed class Installer
{
    #region Fields

    readonly BepInExHelper _bepInEx;
    readonly List<InstallerComponent> _components = new ();

    #endregion

    #region Constructors

    public Installer(BepInExHelper bepInEx)
    {
        _bepInEx = bepInEx ?? throw new ArgumentNullException(nameof(bepInEx));
        _components.Add(new AssemblyInstallerComponent(_bepInEx.PluginsDirectory.FullName));
        _components.Add(new AssetsInstallerComponent(_bepInEx.PluginsDirectory.FullName));
    }

    #endregion

    #region Public Methods

    public async ValueTask<PluginFile> InstallAsync(PluginPackage package, CancellationToken cancel)
    {
        foreach (var step in _components)
            await step.InstallAsync(package, cancel);

        var pluginFilePath = Path.Combine(_bepInEx.PluginsDirectory.FullName, package.TargetAssemblyFileName);
        return new PluginFile(pluginFilePath, _bepInEx.CoreDlls);
    }

    public async ValueTask RemoveAsync(PluginFile plugin, CancellationToken cancel)
    {
        foreach (var step in _components)
            await step.RemoveAsync(plugin, cancel);
    }

    #endregion
}