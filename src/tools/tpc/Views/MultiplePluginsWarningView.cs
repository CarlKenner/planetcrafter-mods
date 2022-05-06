using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using Doublestop.Tpc.Internal;
using Doublestop.Tpc.Plugins;
using Doublestop.Tpc.Plugins.Metadata;

namespace Doublestop.Tpc.Views;

internal sealed class MultiplePluginsWarningView : View
{
    #region Fields

    readonly StackLayoutView _innerView;

    #endregion

    #region Constructors

    public MultiplePluginsWarningView(IReadOnlyList<PluginMetadata> plugins)
    {
        _innerView = new StackLayoutView();
        _innerView.Add(new ContentView($"Removing {plugins.Count} {"plugins".Pluralize(plugins.Count)}."));
        _innerView.Add(new ContentView(TextSpan.Empty()));

        var orderedPlugins = plugins
            .OrderBy(p => p.Name, PluginNameComparer.Instance)
            .ThenBy(p => p.AssemblyFileName, NoCaseTrimmedStringComparer.Default);

        foreach (var plugin in orderedPlugins)
            _innerView.Add(new ContentView($"  * {plugin.Name} [{plugin.Version}]"));

        _innerView.Add(new ContentView(TextSpan.Empty()));
        _innerView.Add(new ContentView("Continue? [y/N] "));
    }

    #endregion

    #region Public Methods

    public override void Render(ConsoleRenderer renderer, Region? region = null) =>
        _innerView.Render(renderer, region);

    public override Size Measure(ConsoleRenderer renderer, Size maxSize) =>
        _innerView.Measure(renderer, maxSize);

    #endregion
}