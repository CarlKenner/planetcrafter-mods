using System.Collections.Immutable;
using System.Diagnostics;
using Doublestop.Tpc.Internal;

namespace Doublestop.Tpc.Config;

[DebuggerDisplay("Count = {_values.Count}")]
internal sealed class TpcConfig
{
    #region Fields

    readonly IReadOnlyDictionary<string, string> _values;

    #endregion

    #region Constructors

    public TpcConfig(IEnumerable<KeyValuePair<string, string>>? values)
    {
        _values = values?.DistinctTakeLast(StringComparer.OrdinalIgnoreCase)
                      .ToImmutableDictionary(StringComparer.OrdinalIgnoreCase)
                  ?? ImmutableDictionary<string, string>.Empty;
    }

    #endregion

    #region Properties

    public string? this[string configKey] => GetValueOrDefault(configKey);

    public string? GameDir => this[ConfigKeys.GameDir];

    #endregion

    #region Public Methods

    public string? GetValueOrDefault(string key) => 
        GetValueOrDefault(key, null);

    public string? GetValueOrDefault(string key, string? defaultVal) => 
        _values.GetValueOrDefault(key) ?? defaultVal;

    #endregion
}