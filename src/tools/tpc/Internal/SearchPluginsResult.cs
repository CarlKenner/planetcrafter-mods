namespace Doublestop.Tpc.Plugins;

public sealed record SearchPluginsResult(int TotalPluginCount, int TotalMatchedPluginsCount, IReadOnlyList<Plugin> Plugins)
{
    public IReadOnlyList<Plugin> Plugins { get; init; } = Plugins ?? throw new ArgumentNullException(nameof(Plugins));
}