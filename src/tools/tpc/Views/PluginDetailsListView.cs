using System.Collections.Immutable;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using Doublestop.Tpc.Plugins;

namespace Doublestop.Tpc.Views;

internal sealed class PluginDetailsListView : ItemsView<Plugin>
{
    #region Fields

    readonly StackLayoutView _layout = new();
    readonly bool _showAssets;
    bool _initialized;

    #endregion

    #region Constructors

    public PluginDetailsListView(IEnumerable<Plugin> plugins, bool showAssets)
    {
        if (plugins == null) throw new ArgumentNullException(nameof(plugins));
        _showAssets = showAssets;
        Items = plugins.ToImmutableList();
    }

    #endregion

    #region Public Methods

    public override void Render(ConsoleRenderer renderer, Region? region = null)
    {
        EnsureInitialized();
        _layout.Render(renderer, region);
    }

    public override Size Measure(ConsoleRenderer renderer, Size maxSize)
    {
        EnsureInitialized();
        return _layout.Measure(renderer, maxSize);
    }

    #endregion

    #region Private Methods

    void EnsureInitialized()
    {
        if (_initialized)
            return;

        foreach (var plugin in Items) 
            _layout.Add(new PluginDetailView(plugin, _showAssets));

        _initialized = true;
    }

    void ResetLayout()
    {
        _layout.Clear();
        _initialized = false;
    }

    #endregion
}