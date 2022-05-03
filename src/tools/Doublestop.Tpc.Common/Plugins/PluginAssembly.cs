using System.Collections.Immutable;

namespace Doublestop.Tpc.Plugins;

public sealed class PluginAssembly
{
    #region Constructors

    public PluginAssembly(string assemblyFile, DateTime lastModifiedUtc, IEnumerable<InstalledPlugin>? plugins = null)
    {
        AssemblyPath = Path.GetFullPath(assemblyFile);
        AssemblyFileName = Path.GetFileName(assemblyFile);
        LastModifiedUtc = lastModifiedUtc;
        Plugins = plugins?.ToImmutableArray() ?? ImmutableArray<InstalledPlugin>.Empty;
    }

    #endregion

    #region Properties

    public string AssemblyPath { get; }
    public string AssemblyFileName { get; }
    public DateTime LastModifiedUtc { get; }
    public IReadOnlyList<InstalledPlugin> Plugins { get; }
    public bool Exists => File.Exists(AssemblyPath);

    #endregion

    #region Public Methods

    public override string ToString() => $"{AssemblyFileName}, {nameof(Plugins)} = {Plugins.Count}";

    #endregion
}