namespace Doublestop.Tpc.Config;

internal sealed class ConfigBuilder
{
    #region Fields

    readonly Dictionary<string, string> _values = new(StringComparer.OrdinalIgnoreCase);

    #endregion

    #region Public Methods

    public ConfigBuilder AddDefaults()
    {
        ConfigDefaults.Set(_values);
        return this;
    }

    public ConfigBuilder AddConfigFile(string? filePath) => 
        AddValues(ConfigFile.Read(filePath, out _));

    public ConfigBuilder AddValue(string key, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return this;

        _values[key] = value;
        return this;
    }

    public ConfigBuilder AddValues(IEnumerable<KeyValuePair<string, string>> values)
    {
        if (values == null) throw new ArgumentNullException(nameof(values));
        foreach (var (key, value) in values.Where(p => !string.IsNullOrWhiteSpace(p.Value)))
            _values[key] = value;
        return this;
    }

    public ConfigBuilder Remove(string key) => 
        Remove(new [] {key});

    public ConfigBuilder Remove(string key, string key2, params string[] moreKeys) => 
        Remove(new [] {key, key2}.Concat(moreKeys));

    public ConfigBuilder Remove(IEnumerable<string> keys)
    {
        if (keys == null) throw new ArgumentNullException(nameof(keys));
        foreach (var key in keys)
            _values.Remove(key);
        return this;
    }

    public ConfigBuilder Reset()
    {
        _values.Clear();
        return this;
    }

    public TpcConfig Build() => new(_values);

    #endregion
}