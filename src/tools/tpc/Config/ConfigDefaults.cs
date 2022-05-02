using System.Collections.Immutable;

namespace Thangs.Tpc.Config;

internal static class ConfigDefaults
{
    static readonly IReadOnlyDictionary<string, string> Defaults = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { ConfigKeys.GameDir, "" }
    }.ToImmutableDictionary();

    public static void Apply(IDictionary<string, string> values)
    {
        foreach (var (key, value) in Defaults) 
            values[key] = value;
    }
}