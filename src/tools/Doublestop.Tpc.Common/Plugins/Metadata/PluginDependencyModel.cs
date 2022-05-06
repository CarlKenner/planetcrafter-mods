using System.Diagnostics;
using BepInEx;

namespace Doublestop.Tpc.Plugins.Metadata;

/// <summary>
/// Information about a plugin required for some other plugin to run.
/// </summary>
/// <remarks>
/// So named to be consistent with <seealso cref="PluginInfoModel"/>.
/// </remarks>
[DebuggerDisplay("{Guid}")]
public sealed class PluginDependencyModel : IEquatable<PluginDependencyModel>
{
    const int HardDependencyFlag = 1;
    const int SoftDependencyFlag = 2;

    #region Constructors

    public PluginDependencyModel(PluginGuid guid, string? version)
    {
        Guid = guid;
        Version = version;
    }

    public PluginDependencyModel(PluginGuid guid, int? dependencyFlags = null)
    {
        Guid = guid;
        DependencyFlags = dependencyFlags ?? HardDependencyFlag;
    }

    #endregion

    #region Properties

    /// <summary>
    /// The dependency's <seealso cref="PluginInfoModel"/>.
    /// </summary>
    public PluginGuid Guid { get; }

    /// <summary>
    /// If set, this property returns the minimum version number of the dependency required. 
    /// </summary>
    /// <remarks>
    /// If this property is set, <see cref="DependencyFlags"/> will be <c>null</c>.
    /// </remarks>
    public string? Version { get; }

    /// <summary>
    /// Corresponds to <seealso cref="BepInDependency.DependencyFlags"/>,
    /// if that information is available.
    /// </summary>
    /// <remarks>
    /// If this property is set, <see cref="Version"/> will be <c>null</c>.
    /// </remarks>
    public int? DependencyFlags { get; }

    public bool IsHard => DependencyFlags.HasValue &&
                          // BepInDependency.DependencyFlags is [Flags]
                          (DependencyFlags.Value & HardDependencyFlag) != 0;


    public bool IsSoft => DependencyFlags.HasValue &&
                          // BepInDependency.DependencyFlags is [Flags]
                          (DependencyFlags.Value & SoftDependencyFlag) != 0;

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns <c>true</c> if <see cref="other"/> is an instance
    /// of <see cref="PluginDependencyModel"/> with a <see cref="Guid"/>
    /// value matching this instance.
    /// </summary>
    public bool Equals(PluginDependencyModel? other) =>
        !ReferenceEquals(other, null) &&
        (ReferenceEquals(this, other) || Guid.Equals(other.Guid));

    /// <summary>
    /// Returns <c>true</c> if <see cref="obj"/> is an instance
    /// of <see cref="PluginDependencyModel"/> with a <see cref="Guid"/>
    /// value matching this instance.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) =>
        ReferenceEquals(this, obj) ||
        obj is PluginDependencyModel other && Equals(other);

    /// <summary>
    /// Returns the hash code of the underlying <see cref="Guid"/>.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() => Guid.GetHashCode();

    #endregion
}