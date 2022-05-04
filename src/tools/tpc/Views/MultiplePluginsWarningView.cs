using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using Doublestop.Tpc.Internal;
using Doublestop.Tpc.Plugins;

namespace Doublestop.Tpc.Views;

internal sealed class MultiplePluginsWarningView : View
{
    #region Fields

    readonly StackLayoutView _innerView;

    #endregion

    #region Constructors

    public MultiplePluginsWarningView(PluginFile file)
    {
        if (file == null) throw new ArgumentNullException(nameof(file));
        var plugins = file.Plugins.ToList();
        _innerView = new StackLayoutView();
        _innerView.Add(new ContentView($"{file.Name} contains {plugins.Count} {"plugins".Pluralize(plugins.Count)}."));
        _innerView.Add(new ContentView(TextSpan.Empty()));
        foreach (var plugin in plugins.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase))
            _innerView.Add(new ContentView($"  * {plugin.Name} [{plugin.Version}]"));
        _innerView.Add(new ContentView(TextSpan.Empty()));
        _innerView.Add(new ContentView("Removing this file will remove them all."));
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