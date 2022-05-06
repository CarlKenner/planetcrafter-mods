using System.Collections;
using Doublestop.Tpc.Game;
using Doublestop.Tpc.Plugins.Metadata;

namespace Doublestop.Tpc.Plugins;

/// <summary>
/// Represents plugins installed on the local system.
/// </summary>
/// <remarks>
/// Most <c>get</c> methods return collections. It's possible, though unlikely,
/// two plugin files exist with identical plugin guids and/or names.
/// <br></br>
/// Most of the time, a guid or name match will return 0 or 1 plugins.
/// However, callers must be prepared to handle the possiblity of multiple matches.
/// </remarks>
public sealed class LocalPlugins : IEnumerable<PluginAssembly>
{
    #region Fields

    readonly BepInExHelper _bepInEx;

    #endregion

    #region Constructors

    public LocalPlugins(BepInExHelper bepInEx)
    {
        _bepInEx = bepInEx ?? throw new ArgumentNullException(nameof(bepInEx));
    }

    #endregion

    #region Properties

    /// <summary>
    /// Returns an enumerable collection of plugin assemblies installed locally.
    /// </summary>
    public IEnumerable<PluginAssembly> PluginAssemblies => _bepInEx.EnumerateAssemblies();

    /// <summary>
    /// Returns a flattened collection of plugins across all <see cref="PluginAssemblies"/>.
    /// </summary>
    public IEnumerable<PluginMetadata> AllPlugins => PluginAssemblies.SelectMany(a => a.Plugins);

    /// <summary>
    /// Returns a count of all plugin assemblies.
    /// </summary>
    public int AssemblyCount => PluginAssemblies.Count();

    /// <summary>
    /// Returns a count of plugins across all assemblies.
    /// </summary>
    public int TotalPluginCount => AllPlugins.Count();

    #endregion

    #region Public Methods

    /// <summary>
    /// Loads a plugin assembly by file name, with or without the <c>.dll</c> extension.
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public PluginAssembly? GetAssembly(string assemblyName) => 
        _bepInEx.GetAssembly(assemblyName);

    public IEnumerable<PluginMetadata> FindPluginsByGuid(PluginGuid guid) => 
        AllPlugins.Where(p => p.Guid.Equals(guid));

    public IEnumerable<PluginMetadata> FindPluginsByName(string pluginName) =>
        AllPlugins.Where(p => PluginNameComparer.Instance.Equals(p.Name, pluginName));

    public IEnumerator<PluginAssembly> GetEnumerator() => PluginAssemblies.GetEnumerator();

    #endregion

    #region Private Methods

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion
}