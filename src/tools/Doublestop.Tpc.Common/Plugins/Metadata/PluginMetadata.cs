using System.Collections.Immutable;

namespace Doublestop.Tpc.Plugins.Metadata;

public sealed class PluginMetadata
{
    public PluginMetadata(string assemblyFileName, PluginInfoModel info, IEnumerable<PluginDependencyModel>? dependencies)
    {
        AssemblyFileName = assemblyFileName;
        Info = info ?? throw new ArgumentNullException(nameof(info));
        Dependencies = dependencies?.ToImmutableList() ?? 
                       ImmutableList<PluginDependencyModel>.Empty;
    }

    public string AssemblyFileName { get; }

    public PluginInfoModel Info { get; }

    /// <summary>
    /// The plugin's unique id, as defined by the plugin author.
    /// </summary>
    /// <remarks>
    /// Convenience pass-through to <see cref="Info" />.
    /// </remarks>
    public PluginGuid Guid => Info.Guid;

    /// <summary>
    /// The plugin's name, as defined by the plugin author.
    /// </summary>
    /// <remarks>
    /// Convenience pass-through to <see cref="Info" />.
    /// </remarks>
    public string Name => Info.Name;

    /// <summary>
    /// The plugin's version, as defined by the plugin author.
    /// </summary>
    /// <remarks>
    /// Convenience pass-through to <see cref="Info" />.
    /// </remarks>
    public string Version => Info.Version;

    /// <summary>
    /// List of plugins required for this plugin to function.
    /// </summary>
    public IReadOnlyList<PluginDependencyModel> Dependencies { get; }
}