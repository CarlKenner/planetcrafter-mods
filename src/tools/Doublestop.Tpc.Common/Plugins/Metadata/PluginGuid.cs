using Doublestop.Tpc.Internal;

namespace Doublestop.Tpc.Plugins.Metadata;

/// <summary>
/// Contains a plugin's guid.
/// </summary>
/// <remarks>
/// I'm my own type because plugin guids have a tiny bit of logic to them.
/// </remarks>
public readonly struct PluginGuid : IEquatable<PluginGuid>
{
    #region Constructors

    public PluginGuid(string guid)
    {
        if (string.IsNullOrWhiteSpace(guid))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(guid));

        Value = guid;
    }

    #endregion

    #region Properties

    public string Value { get; }

    #endregion

    #region Public Methods

    public static implicit operator string(PluginGuid guid) => guid.Value;

    public static implicit operator PluginGuid(string value) => new(value);

    public static bool operator ==(PluginGuid a, PluginGuid b) =>
        PluginGuidComparer.Instance.Equals(a, b);

    public static bool operator !=(PluginGuid a, PluginGuid b) => !(a == b);

    public override string ToString() => Value;

    public bool Equals(PluginGuid other) => 
        PluginGuidComparer.Instance.Equals(this, other);

    public override bool Equals(object? obj) =>
        obj is PluginGuid other && Equals(other);

    public override int GetHashCode() => 
        PluginGuidComparer.Instance.GetHashCode(this);

    #endregion
}