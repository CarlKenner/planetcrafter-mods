using System.Collections;
using System.Collections.Immutable;
using Doublestop.Tpc.Game;
using Doublestop.Tpc.Plugins.Installing;

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
public sealed class LocalPlugins : IEnumerable<PluginFile>
{
    #region Fields

    readonly BepInExHelper _bepInEx;

    #endregion

    #region Constructors

    public LocalPlugins(BepInExHelper bepInEx)
    {
        _bepInEx = bepInEx ?? throw new ArgumentNullException(nameof(bepInEx));
        Installer = new Installer(_bepInEx);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Returns all the files that might contain a plugin.
    /// </summary>
    /// <remarks>
    /// This is essentially a list of <c>dll</c> files, not necessarily a list of valid plugin assemblies.
    /// <br></br>
    /// The number of plugin files will not necessarily equal the number of plugins
    /// the game discovers and loads. If any plugin files contain more that one plugin (or none),
    /// or any plugin file fails to load (see <see cref="PlugFileLoadError"/>), the counts will certainly disagree.
    /// </remarks>
    public IEnumerable<PluginFile> PluginFiles =>
        _bepInEx.PluginDlls.Select(path => new PluginFile(path, _bepInEx.CoreDlls));

    /// <summary>
    /// Returns a flattened collection of plugins across all <see cref="PluginFiles"/>.
    /// </summary>
    /// <remarks>
    /// This is a safe enumerator, returning only those plugins which loaded successfully.
    /// <br></br>
    /// Subscribe to the <see cref="PlugFileLoadError"/> event to be notified of load errors.
    /// <br></br>
    /// This enumerator is not cached, and is renewed with each read.
    /// Take a list of this property if you wish to work with a snapshot.
    /// </remarks>
    public IEnumerable<Plugin> Plugins => EnumeratePlugins();

    public Installer Installer { get; }

    #endregion

    #region Events

    public event Action<PluginFile, Exception>? PlugFileLoadError;

    #endregion

    #region Public Methods

    public ValueTask<PluginFile?> GetFileByName(string filename, CancellationToken cancel)
    {
        if (filename == null) throw new ArgumentNullException(nameof(filename));
        return ValueTask.FromResult(TryLoad());
        PluginFile? TryLoad()
        {
            var fullPath = Path.Combine(_bepInEx.PluginsDirectory.FullName, filename);
            return File.Exists(fullPath)
                ? new PluginFile(fullPath, _bepInEx.CoreDlls)
                : null;
        }
    }

    /// <summary>
    /// Returns plugins matching the specified guid.
    /// </summary>
    /// <param name="pluginGuid"></param>
    /// <param name="cancel"></param>
    /// <returns></returns>
    public ValueTask<IReadOnlyList<Plugin>> GetPluginsByGuidAsync(string pluginGuid, CancellationToken cancel)
    {
        if (pluginGuid == null) throw new ArgumentNullException(nameof(pluginGuid));
        var list = Plugins
            .Where(p => PluginGuidComparer.Instance.Equals(pluginGuid, p.Guid))
            .ToImmutableList();

        return ValueTask.FromResult<IReadOnlyList<Plugin>>(list);
    }

    public ValueTask<IReadOnlyList<Plugin>> GetPluginsByNameAsync(string name, CancellationToken cancel)
    {
        if (name == null) throw new ArgumentNullException(nameof(name));
        var list = Plugins
            .Where(p => PluginNameComparer.Instance.Equals(name, p.Name))
            .ToImmutableList();

        return ValueTask.FromResult<IReadOnlyList<Plugin>>(list);
    }

    public IEnumerator<PluginFile> GetEnumerator() => PluginFiles.GetEnumerator();

    #endregion

    #region Private Methods

    IEnumerable<Plugin> EnumeratePlugins()
    {
        foreach (var file in PluginFiles)
        {
            // Can't yield from a try/catch, so use an intermediary local
            // that will return a null enumerator in the event of a load error.
            var plugins = TryLoadPlugins(file);
            if (plugins is null)
                // error occurred, event notified.
                continue;

            // plugin file loaded, now combine info + file into an all powerful collection of plugins.
            foreach (var info in plugins)
                yield return new Plugin(info, file);
        }

        IEnumerable<PluginInfo>? TryLoadPlugins(PluginFile file)
        {
            try
            {
                return file.ToList();
            }
            catch (Exception ex)
            {
                try
                {
                    PlugFileLoadError?.Invoke(file, ex);
                }
                catch
                {
                    // ignored
                }
                return null;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion
}