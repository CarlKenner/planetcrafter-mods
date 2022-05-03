using System.Collections.Immutable;

namespace Doublestop.Tpc.Config;

internal static class ConfigDefaults
{
    #region Fields

    static readonly IReadOnlyDictionary<string, string> Defaults = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { ConfigKeys.GameDir, "" }
    }.ToImmutableDictionary();

    #endregion

    #region Public Methods

    public static void Set(IDictionary<string, string> values)
    {
        foreach (var (key, value) in Defaults) 
            values[key] = value;
    }

    #endregion
}