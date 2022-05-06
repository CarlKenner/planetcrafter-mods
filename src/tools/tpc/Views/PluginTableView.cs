using System.Collections.Immutable;
using System.CommandLine.Rendering.Views;
using Doublestop.Tpc.Plugins.Metadata;

namespace Doublestop.Tpc.Views;

internal sealed class PluginTableView : TableView<PluginMetadata>
{
    #region Constructors

    public PluginTableView(IEnumerable<PluginMetadata> plugins)
    {
        if (plugins == null) throw new ArgumentNullException(nameof(plugins));
        // todo: localization
        AddColumn(t => t.Name, "Name", ColumnDefinition.Fixed(50));
        AddColumn(t => t.Version, "Version", ColumnDefinition.Fixed(12));
        AddColumn(t => t.AssemblyFileName, "File", ColumnDefinition.Star(.33d));
        Items = plugins.ToImmutableList();
    }

    #endregion
}