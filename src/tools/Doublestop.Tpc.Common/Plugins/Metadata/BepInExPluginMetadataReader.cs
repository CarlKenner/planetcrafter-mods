using System.Collections.Immutable;
using Doublestop.Tpc.Internal;

namespace Doublestop.Tpc.Plugins.Metadata;

public class BepInExPluginMetadataReader : PluginMetadataReader
{
    #region Fields

    readonly IReadOnlyList<string> _resolverPaths;

    #endregion

    #region Constructors

    public BepInExPluginMetadataReader(IEnumerable<string>? resolverPaths = null)
    {
        if (resolverPaths == null)
            throw new ArgumentNullException(nameof(resolverPaths));

        _resolverPaths = resolverPaths.ToImmutableList();
    }

    #endregion

    #region Properties

    public bool IncludeSysCoreLibs { get; init; } = true;

    #endregion

    #region Public Methods

    public override IEnumerable<PluginMetadata> Read(string assemblyFile)
    {
        using var loaded = LoadedAssembly.Load(assemblyFile, _resolverPaths, IncludeSysCoreLibs);
        foreach (var type in loaded.Assembly.GetTypes())
            if (Read(type) is {} metadata)
                yield return metadata;
    }

    #endregion

    #region Protected Methods

    protected override PluginMetadata? OnRead(Type type)
    {
        if (type.IsAbstract || !type.IsClass)
            return null;

        return BepInAttributes.ReadPluginMetadata(type);
    }

    #endregion
}