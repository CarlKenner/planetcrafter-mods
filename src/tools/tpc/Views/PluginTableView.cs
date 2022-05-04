using System.Collections.Immutable;
using System.CommandLine.Rendering.Views;
using Doublestop.Tpc.Plugins;

namespace Doublestop.Tpc.Views;

internal sealed class PluginTableView : TableView<Plugin>
{
    #region Constructors

    public PluginTableView(IEnumerable<Plugin> plugins)
    {
        if (plugins == null) throw new ArgumentNullException(nameof(plugins));
        // todo: localization
        AddColumn(t => t.Name, "Name", ColumnDefinition.Fixed(50));
        AddColumn(t => t.Version, "Version", ColumnDefinition.Fixed(12));
        AddColumn(t => t.File.Name, "File", ColumnDefinition.Star(.33d));
        Items = plugins.ToImmutableList();
    }

    #endregion
}