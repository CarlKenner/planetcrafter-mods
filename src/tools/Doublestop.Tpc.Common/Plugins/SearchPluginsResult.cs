namespace Doublestop.Tpc.Plugins;

public sealed record SearchPluginsResult(int TotalPluginCount, int TotalMatchedPluginsCount, IReadOnlyList<InstalledPlugin> Plugins)
{
    public int Count => Plugins.Count;

    public IReadOnlyList<InstalledPlugin> Plugins { get; init; } = Plugins ?? throw new ArgumentNullException(nameof(Plugins));
}