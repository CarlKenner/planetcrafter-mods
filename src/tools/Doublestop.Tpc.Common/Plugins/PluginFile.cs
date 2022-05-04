using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics;
using Doublestop.Tpc.Internal;

namespace Doublestop.Tpc.Plugins;

[DebuggerDisplay("{Name}")]
public sealed class PluginFile : IEquatable<PluginFile>, IEnumerable<PluginInfo>
{
    #region Fields

    readonly IReadOnlyList<string> _dependencies;

    readonly FileInfo _file;

    #endregion

    #region Constructors

    public PluginFile(string path, IEnumerable<string> coreDlls)
    {
        if (coreDlls == null) throw new ArgumentNullException(nameof(coreDlls));
        _file = new FileInfo(path);
        _dependencies = coreDlls.ToImmutableList();
    }

    #endregion

    #region Properties

    public string Path => _file.FullName;
    public string Name => _file.Name;
    public string? Directory => _file.DirectoryName;
    public string NameWithoutExtension => System.IO.Path.GetFileNameWithoutExtension(Name);
    public bool Exists => _file.Exists;
    public DateTime LastModifiedUtc => _file.LastWriteTimeUtc;

    public IEnumerable<PluginInfo> Plugins => EnumeratePlugins();

    #endregion

    #region Public Methods

    public bool Equals(PluginFile? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return string.Equals(Path, other.Path, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) =>
        ReferenceEquals(this, obj) || 
        obj is PluginFile other && Equals(other);


    public override int GetHashCode() => Path.GetHashCode(StringComparison.OrdinalIgnoreCase);

    public IEnumerator<PluginInfo> GetEnumerator() => Plugins.GetEnumerator();

    public void Delete()
    {
        if (_file.Exists)
            _file.Delete();
    }

    #endregion

    #region Private Methods

    IEnumerable<PluginInfo> EnumeratePlugins()
    {
        using var loaded = Reflect.LoadAssembly(Path, _dependencies);
        foreach (var type in loaded.Assembly.GetTypes())
            if (PluginInfo.Get(type) is { } info)
                yield return info;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion
}