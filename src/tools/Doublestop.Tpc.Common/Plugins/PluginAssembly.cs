using System.Collections;
using System.Diagnostics;
using Doublestop.Tpc.Plugins.Metadata;

namespace Doublestop.Tpc.Plugins;

[DebuggerDisplay("{Name}")]
public sealed class PluginAssembly : IEquatable<PluginAssembly>, IEnumerable<PluginMetadata>
{
    #region Fields

    readonly PluginMetadataReader _metadataReader;

    #endregion

    #region Constructors

    public PluginAssembly(string path, IEnumerable<string> bepInExCoreDlls) 
        : this(path, PluginMetadataReader.Create(bepInExCoreDlls))
    {
    }

    public PluginAssembly(string path, PluginMetadataReader metadataReader)
    {
        _metadataReader = metadataReader ?? throw new ArgumentNullException(nameof(metadataReader));
        AssemblyFile = new FileInfo(path);
    }

    #endregion

    #region Properties

    public FileInfo AssemblyFile { get; }
    public string Path => AssemblyFile.FullName;
    public string Name => AssemblyFile.Name;
    public string? Directory => AssemblyFile.DirectoryName;
    public string NameWithoutExtension => System.IO.Path.GetFileNameWithoutExtension(Name);
    public bool Exists => AssemblyFile.Exists;

    public IEnumerable<PluginMetadata> Plugins => _metadataReader.Read(Path);

    #endregion

    #region Public Methods

    public bool Delete()
    {
        if (!AssemblyFile.Exists)
            return false;

        AssemblyFile.Delete();
        return true;
    }

    public bool Equals(PluginAssembly? other) =>
        !ReferenceEquals(null, other) && 
        (ReferenceEquals(this, other) ||
         string.Equals(Path, other.Path, StringComparison.OrdinalIgnoreCase));

    public override bool Equals(object? obj) =>
        ReferenceEquals(this, obj) ||
        obj is PluginAssembly other && Equals(other);

    public override int GetHashCode() => Path.GetHashCode(StringComparison.OrdinalIgnoreCase);

    public IEnumerator<PluginMetadata> GetEnumerator() => Plugins.GetEnumerator();

    #endregion

    #region Private Methods

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion
}