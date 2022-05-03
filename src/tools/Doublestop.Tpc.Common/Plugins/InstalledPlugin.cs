using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using BepInEx;
using Doublestop.Tpc.Internal;

namespace Doublestop.Tpc.Plugins;

[DebuggerDisplay("{Name} => {AssemblyFile.Name}")]
public sealed class InstalledPlugin
{
    #region Constructors

    public InstalledPlugin(string guid, string name, string version, FileInfo assemblyFile, string? assetsDirectory = null)
    {
        Guid = guid ?? throw new ArgumentNullException(nameof(guid));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Version = version ?? throw new ArgumentNullException(nameof(version));
        AssemblyFile = assemblyFile;

        assetsDirectory ??= Path.GetFileNameWithoutExtension(AssemblyFile.Name);
        if (!Path.IsPathRooted(assetsDirectory) && assemblyFile.DirectoryName is not null)
            assetsDirectory = Path.Combine(assemblyFile.DirectoryName, assetsDirectory);
        AssetsDirectory = new DirectoryInfo(assetsDirectory);
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

    /// <summary>
    /// The plugin's assembly file (.dll).
    /// </summary>
    public FileInfo AssemblyFile { get; }

    /// <summary>
    /// The last modified date of the assembly containing this plugin.
    /// </summary>
    public DateTime LastModifiedUtc => AssemblyFile.LastWriteTimeUtc;

    /// <summary>
    /// The plugin's assets directory, which may or may not exist.
    /// </summary>
    public DirectoryInfo AssetsDirectory { get; }

    #endregion

    #region Public Methods

    public static IEnumerable<InstalledPlugin> GetPlugins(Assembly assembly)
    {
        if (assembly == null) throw new ArgumentNullException(nameof(assembly));
        return GetPlugins(assembly.GetTypes());
    }

    public static IEnumerable<InstalledPlugin> GetPlugins(IEnumerable<Type> types)
    {
        if (types == null) throw new ArgumentNullException(nameof(types));
        return types.Select(TryGetInfo).WhereNotNull();
    }

    public static InstalledPlugin? TryGetInfo(Type type)
    {

        var assemblyFile = new FileInfo(type.Assembly.Location);
        return TryGetInfo(type, out var guid, out var name, out var version)
            ? new InstalledPlugin(guid, name, version, assemblyFile)
            : null;
    }

    public static bool TryGetInfo(Type type, [NotNullWhen(true)] out string? guid,
        [NotNullWhen(true)] out string? name, [NotNullWhen(true)] out string? version)
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
            guid = attribute.ConstructorArguments[0].Value?.ToString() ?? "";
            // name (eg "dude's freakishly cool mod")
            name = attribute.ConstructorArguments[1].Value?.ToString() ?? "";
            // version (eg 1.2.3.4)
            version = attribute.ConstructorArguments[2].Value?.ToString() ?? "";
            return true;
        }
        guid = name = version = default;
        return false;
    }

    #endregion

    #region Private Methods

    static bool IsStringType(Type argumentType) => string.Equals(argumentType.FullName, typeof(string).FullName);

    #endregion
}