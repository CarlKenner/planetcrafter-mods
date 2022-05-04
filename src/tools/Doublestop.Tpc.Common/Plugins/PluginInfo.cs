using System.Reflection;
using BepInEx;
using Doublestop.Tpc.Internal;

namespace Doublestop.Tpc.Plugins;

public sealed class PluginInfo : IEquatable<PluginInfo>
{
    #region Constructors

    public PluginInfo(string guid, string name, string version)
    {
        if (guid == null) throw new ArgumentNullException(nameof(guid));
        if (name == null) throw new ArgumentNullException(nameof(name));
        if (version == null) throw new ArgumentNullException(nameof(version));

        Guid = guid.Trim();
        Name = name.Trim();
        Version = version.Trim();
    }

    #endregion

    #region Properties

    /// <summary>
    /// The plugin's unique id, as defined by the plugin author.
    /// </summary>
    public string Guid { get; }

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

    public bool Equals(PluginInfo? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return PluginGuidComparer.Instance.Equals(Guid, other.Guid);
    }

    public override bool Equals(object? obj) =>
        ReferenceEquals(this, obj) ||
        obj is PluginInfo other && Equals(other);

    public override int GetHashCode() => PluginGuidComparer.Instance.GetHashCode(Guid);

    public static IEnumerable<PluginInfo> Get(Assembly assembly)
    {
        if (assembly == null) throw new ArgumentNullException(nameof(assembly));
        return Get(assembly.GetTypes());
    }

    public static IEnumerable<PluginInfo> Get(IEnumerable<Type> types)
    {
        if (types == null) throw new ArgumentNullException(nameof(types));
        return types.Select(Get).WhereNotNull();
    }

    public static PluginInfo? Get(Type type)
    {
        foreach (var attribute in CustomAttributeData.GetCustomAttributes(type))
        {
            if (!string.Equals(attribute.AttributeType.Name, nameof(BepInPlugin)))
                // not a [BepInPlugin] attribute
                continue;

            if (attribute.ConstructorArguments.Count != 3 ||
                attribute.ConstructorArguments.Any(ca => !IsStringType(ca.ArgumentType)))
                // expecting the (guid, name, version) constructor params
                continue;

            // guid (eg "dudes_freakishly_cool_mod")
            // guids are regex restricted to a-z, A-Z, 0-9, -, _
            var guid = attribute.ConstructorArguments[0].Value?.ToString() ?? "";
            // name (eg "dude's freakishly cool mod")
            var name = attribute.ConstructorArguments[1].Value?.ToString() ?? "";
            // version (eg 1.2.3.4)
            var version = attribute.ConstructorArguments[2].Value?.ToString() ?? "";

            return new PluginInfo(guid, name, version);
        }
        return null;

        static bool IsStringType(Type argumentType) => string.Equals(argumentType.FullName, typeof(string).FullName);
    }

    #endregion
}