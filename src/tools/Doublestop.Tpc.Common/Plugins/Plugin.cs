using System.Diagnostics;
using Doublestop.Tpc.Plugins.Metadata;

namespace Doublestop.Tpc.Plugins;

[DebuggerDisplay("{Name} => {Assembly}")]
public sealed class Plugin : IEquatable<Plugin>
{
    #region Constructors

    public Plugin(PluginAssembly assembly, PluginMetadata metadata, string? assetsDirectoryName = null)
    {
        Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
        Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));

        // default asset dir is assembly filename without extension, eg "my-mod.dll" -> "my-mod/"
        assetsDirectoryName ??= Assembly.NameWithoutExtension;
        // if the given asset dir is relative, combine it with the the plugin assembly's containing directory.
        if (!Path.IsPathRooted(assetsDirectoryName) && Assembly.Directory is not null)
            assetsDirectoryName = Path.Combine(Assembly.Directory, assetsDirectoryName);
        AssetsDirectory = new DirectoryInfo(assetsDirectoryName);
    }

    #endregion

    #region Properties

    /// <summary>
    /// The plugin's containing assembly.
    /// </summary>
    public PluginAssembly Assembly { get; }

    /// <summary>
    /// The plugin's metadata (info, dependencies, etc)
    /// </summary>
    public PluginMetadata Metadata { get; }

    /// <summary>
    /// The plugin's unique id, as defined by the plugin author.
    /// </summary>
    /// <remarks>
    /// Convenience pass-through to <see cref="Metadata"/>.<see cref="PluginMetadata.Guid" />.
    /// </remarks>
    public PluginGuid Guid => Metadata.Guid;

    /// <summary>
    /// The plugin's name, as defined by the plugin author.
    /// </summary>
    /// <remarks>
    /// Convenience pass-through to <see cref="Metadata"/>.<see cref="PluginMetadata.Name" />.
    /// </remarks>
    public string Name => Metadata.Name;

    /// <summary>
    /// The plugin's version, as defined by the plugin author.
    /// </summary>
    /// <remarks>
    /// Convenience pass-through to <see cref="Metadata"/>.<see cref="PluginMetadata.Version" />.
    /// </remarks>
    public string Version => Metadata.Version;

    /// <summary>
    /// The plugin's assets directory, which may or may not exist.
    /// </summary>
    public DirectoryInfo AssetsDirectory { get; }

    #endregion

    #region Public Methods

    public bool Equals(Plugin? other) =>
        !ReferenceEquals(null, other) && 
        (ReferenceEquals(this, other) ||
         Metadata.Info.Equals(other.Metadata.Info) && Assembly.Equals(other.Assembly));

    public override bool Equals(object? obj) => 
        ReferenceEquals(this, obj) || 
        obj is Plugin other && Equals(other);

    public override int GetHashCode() => 
        HashCode.Combine(Metadata.Info, Assembly);

    #endregion
}