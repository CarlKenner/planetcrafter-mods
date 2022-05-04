using System.Diagnostics;

namespace Doublestop.Tpc.Plugins;

[DebuggerDisplay("{Name} => {File}")]
public sealed class Plugin : IEquatable<Plugin>
{
    #region Constructors

    public Plugin(PluginInfo info, PluginFile file, string? assetsDirectory = null)
    {
        Info = info ?? throw new ArgumentNullException(nameof(info));
        File = file ?? throw new ArgumentNullException(nameof(file));

        // default asset dir is assembly filename without extension, eg "my-mod.dll" -> "my-mod/"
        assetsDirectory ??= File.NameWithoutExtension;
        // if the given asset dir is relative, combine it with the the plugin assembly's containing directory.
        if (!Path.IsPathRooted(assetsDirectory) && File.Directory is not null)
            assetsDirectory = Path.Combine(File.Directory, assetsDirectory);
        AssetsDirectory = new DirectoryInfo(assetsDirectory);
    }

    #endregion

    #region Properties

    public PluginInfo Info { get; }

    /// <summary>
    /// The plugin's unique id, as defined by the plugin author.
    /// </summary>
    public string Guid => Info.Guid;

    /// <summary>
    /// The plugin's name, as defined by the plugin author.
    /// </summary>
    public string Name => Info.Name;

    /// <summary>
    /// The plugin's version, as defined by the plugin author.
    /// </summary>
    public string Version => Info.Version;

    /// <summary>
    /// The plugin's assembly file (.dll).
    /// </summary>
    public PluginFile File { get; }

    /// <summary>
    /// The plugin's assets directory, which may or may not exist.
    /// </summary>
    public DirectoryInfo AssetsDirectory { get; }

    #endregion

    #region Public Methods

    #endregion

    public bool Equals(Plugin? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Info.Equals(other.Info) && File.Equals(other.File);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is Plugin other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Info, File);
    }
}