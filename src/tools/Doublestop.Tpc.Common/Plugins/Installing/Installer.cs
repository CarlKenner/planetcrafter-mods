using Doublestop.Tpc.Plugins.Installing.Components;

namespace Doublestop.Tpc.Plugins.Installing;

public sealed class Installer
{
    #region Fields

    readonly List<InstallerComponent> _components = new ();

    #endregion

    #region Constructors

    public Installer(string pluginsDirectory)
    {
        if (string.IsNullOrWhiteSpace(pluginsDirectory))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(pluginsDirectory));
            
        _components.Add(new AssemblyInstallerComponent(pluginsDirectory));
        _components.Add(new AssetsInstallerComponent(pluginsDirectory));
    }

    #endregion

    #region Public Methods

    public async ValueTask InstallAsync(PluginPackage package, CancellationToken cancel)
    {
        foreach (var step in _components)
            await step.InstallAsync(package, cancel);
    }

    public async ValueTask RemoveAsync(InstalledPlugin plugin, CancellationToken cancel)
    {
        foreach (var step in _components)
            await step.RemoveAsync(plugin, cancel);
    }

    #endregion
}