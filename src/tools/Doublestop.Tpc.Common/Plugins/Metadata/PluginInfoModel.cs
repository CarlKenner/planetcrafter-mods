using Doublestop.Tpc.Internal;

namespace Doublestop.Tpc.Plugins.Metadata;

/// <summary>
/// Contains the guid, name, and version of a plugin.
/// </summary>
/// <remarks>
/// So named to avoid clashing with <seealso cref="BepInEx.PluginInfo"/>
/// </remarks>
public sealed class PluginInfoModel : IEquatable<PluginInfoModel>
{
    #region Constructors

    public PluginInfoModel(PluginGuid guid, string name, string version)
    {
        if (name == null) throw new ArgumentNullException(nameof(name));
        if (version == null) throw new ArgumentNullException(nameof(version));

        Guid = guid;
        Name = name.Trim();
        Version = version.Trim();
    }

    #endregion

    #region Properties

    /// <summary>
    /// The plugin's unique id, as defined by the plugin author.
    /// </summary>
    public PluginGuid Guid { get; }

    /// <summary>
    /// The plugin's name, as defined by the plugin author.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The plugin's version, as defined by the plugin author.
    /// </summary>
    public string Version { get; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns <see cref="Name"/>.
    /// </summary>
    public override string ToString() => Name;

    public bool Equals(PluginInfoModel? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return PluginGuidComparer.Instance.Equals(Guid, other.Guid);
    }

    public override bool Equals(object? obj) =>
        ReferenceEquals(this, obj) ||
        obj is PluginInfoModel other && Equals(other);

    public override int GetHashCode() => PluginGuidComparer.Instance.GetHashCode(Guid);

    #endregion
}