using Doublestop.Tpc.Plugins;

namespace Doublestop.Tpc.Internal;

public sealed record SearchPluginsResult(int TotalPluginCount, int TotalMatchedPluginsCount, IReadOnlyList<Plugin> Plugins)
{
    public IReadOnlyList<Plugin> Plugins { get; init; } = Plugins ?? throw new ArgumentNullException(nameof(Plugins));
}