using Doublestop.Tpc.Game;

namespace Doublestop.Tpc.Plugins.Metadata;

/// <summary>
/// Reads plugin metadata from plugin assemblies and types.
/// </summary>
public abstract class PluginMetadataReader
{
    /// <summary>
    /// Invoked by <see cref="Read(string)"/>, for types in the assembly which fail to load.
    /// </summary>
    public event Action<Type, Exception>? TypeReadError;

    public abstract IEnumerable<PluginMetadata> Read(string assemblyFile);

    public PluginMetadata? Read(Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        try
        {
            return OnRead(type);
        }
        catch (Exception ex)
        {
            TypeReadError?.Invoke(type, ex);
            return null;
        }
    }

    protected abstract PluginMetadata? OnRead(Type type);

    public static PluginMetadataReader Create(BepInExHelper bepInEx) => 
        Create(bepInEx.CoreDlls);

    public static PluginMetadataReader Create(IEnumerable<string> bepInExCoreDlls) => 
        new BepInExPluginMetadataReader(bepInExCoreDlls);
}