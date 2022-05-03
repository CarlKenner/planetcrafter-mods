using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Doublestop.Tpc.Internal;
using Doublestop.Tpc.Plugins.Installing;

namespace Doublestop.Tpc.Plugins;

public sealed class LocalPlugins
{
    #region Fields

    readonly ThePlanetCrafter _game;

    #endregion

    #region Constructors

    public LocalPlugins(ThePlanetCrafter game)
    {
        _game = game ?? throw new ArgumentNullException(nameof(game));
    }

    #endregion

    #region Properties

    /// <summary>
    /// Returns all the files that might contain a plugin.
    /// </summary>
    public IEnumerable<FileInfo> PluginFiles =>
        _game.BepInEx.PluginsDirectory.EnumerateFiles("*.dll", SearchOption.TopDirectoryOnly);

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns the count of installed plugins.
    /// </summary>
    /// <param name="cancel"></param>
    /// <returns></returns>
    public async ValueTask<int> CountAsync(CancellationToken cancel)
    {
        var tasks = PluginFiles.Select(async f => await GetAsync(f.Name, cancel));
        var files = await Task.WhenAll(tasks);
        return files.Length;
    }

    /// <summary>
    /// Returns a list of plugins matching the search request.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancel"></param>
    /// <returns></returns>
    public async ValueTask<SearchPluginsResult> SearchAsync(SearchPluginsRequest? request, CancellationToken cancel)
    {
        request ??= SearchPluginsRequest.Empty;
        var allAssemblies = await GetAllPluginsAsync(cancel);
        var totalCount = allAssemblies
            .Select(a => a.Plugins.Count)
            .DefaultIfEmpty(0)
            .Sum();
        var filteredList = ApplySearchFilters(request, allAssemblies).ToList();
        var matchedCount = filteredList.Count;
        return new SearchPluginsResult(totalCount, matchedCount, filteredList);
    }

    /// <summary>
    /// Installs a plugin.
    /// </summary>
    /// <param name="package"></param>
    /// <param name="cancel"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    public async ValueTask<PluginAssembly> AddAsync(PluginPackage package, CancellationToken cancel)
    {
        var installer = new Installer(_game.BepInEx.PluginsDirectory.FullName);
        await installer.InstallAsync(package, cancel);
        return await GetAsync(package.TargetAssemblyFileName, cancel) ??
               throw new FileNotFoundException($"Plugin {package.TargetAssemblyFileName} was not found after installation.");
    }

    /// <summary>
    /// Removes an installed plugin.
    /// </summary>
    /// <param name="assemblyFileName"></param>
    /// <param name="cancel"></param>
    /// <returns></returns>
    public async ValueTask RemoveAsync(string assemblyFileName, CancellationToken cancel)
    {
        var installer = new Installer(_game.BepInEx.PluginsDirectory.FullName);
        await installer.RemoveAsync(assemblyFileName, cancel);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="assemblyFileName"></param>
    /// <param name="cancel"></param>
    /// <returns></returns>
    public async ValueTask<PluginAssembly?> GetAsync(string assemblyFileName, CancellationToken cancel)
    {
        return await Task.Run(() =>
        {
            // If assemblyFileName is relative, it's probably just a filename.
            // Whatever the case, relative paths are rooted to the PluginsDirectory.
            if (!Path.IsPathRooted(assemblyFileName))
                assemblyFileName = Path.Combine(_game.BepInEx.PluginsDirectory.FullName, assemblyFileName);

            if (!File.Exists(assemblyFileName))
                return null;

            using var context = new MetadataLoadContext(
                new PathAssemblyResolver(
                    GetCommonResolverPaths().Prepend(assemblyFileName)));
            var assembly = context.LoadFromAssemblyPath(assemblyFileName);
            return new PluginAssembly(
                assemblyFileName,
                File.GetLastWriteTimeUtc(assemblyFileName),
                InstalledPlugin.GetPlugins(assembly));
        }, cancel);

        #region Local Helpers

        IEnumerable<string> GetCommonResolverPaths()
        {
            // Todo: Allow configuration of alternate/additional resolver paths

            // For now, the following paths are searched:
            // * .../System.Private.CoreLib.dll
            // * .../mscorlib.dll
            // * .../The Planet Crafter/BepInEx/core/*.dll

            var runtimeDirectory = RuntimeEnvironment.GetRuntimeDirectory();
            var systemPrivateCoreLib = Path.Combine(runtimeDirectory, "System.Private.CoreLib.dll");
            var msCorLib = Path.Combine(runtimeDirectory, "mscorlib.dll");

            yield return systemPrivateCoreLib;
            yield return msCorLib;
            foreach (var file in _game.BepInEx.CoreDlls)
                yield return file.FullName;
        }

        #endregion
    }

    #endregion

    #region Private Methods

    async ValueTask<IReadOnlyList<PluginAssembly>> GetAllPluginsAsync(CancellationToken cancel)
    {
        var tasks = PluginFiles.Select(async f => await GetAsync(f.Name, cancel));
        var assemblies = await Task.WhenAll(tasks);
        return assemblies.WhereNotNull().ToImmutableList();
    }

    static IEnumerable<InstalledPlugin> ApplySearchFilters(SearchPluginsRequest request, IEnumerable<PluginAssembly> allAssemblies)
    {
        var plugins = FlattenAssembliesToPlugins(allAssemblies);

        // Search terms
        if (request.SearchTerms.Any())
        {
            if (request.UseRegex)
            {
                var regexList = request.SearchTerms.Distinct().Select(s => new Regex(s, RegexOptions.IgnoreCase)).ToArray();
                if (request.MatchAllTerms)
                    plugins = plugins.Where(p => regexList.All(r =>
                    r.IsMatch(p.Name) ||
                    r.IsMatch(p.AssemblyFile.Name)));
                else
                    plugins = plugins.Where(p => regexList.Any(r =>
                        r.IsMatch(p.Name) ||
                        r.IsMatch(p.AssemblyFile.Name)));
            }
            else
            {
                var matcher = new StringPatternMatcher(true);
                if (request.MatchAllTerms)
                    plugins = plugins.Where(p => request.SearchTerms.All(t =>
                        matcher.IsMatch(p.Name, t) ||
                        matcher.IsMatch(p.AssemblyFile.Name, t)));
                else
                    plugins = plugins.Where(p => request.SearchTerms.Any(t =>
                        matcher.IsMatch(p.Name, t) ||
                        matcher.IsMatch(p.AssemblyFile.Name, t)));
            }
        }

        // tbd: more filters (last modified, version, etc)

        return plugins;
    }

    /// <summary>
    /// Always orders plugins by assembly file name, then by the names of the plugins (typically 1) contained in that assembly.
    /// </summary>
    /// <param name="pluginAssemblies"></param>
    /// <returns></returns>
    static IEnumerable<InstalledPlugin> FlattenAssembliesToPlugins(IEnumerable<PluginAssembly> pluginAssemblies) =>
        pluginAssemblies
            .OrderBy(a => a.AssemblyFileName, StringComparer.OrdinalIgnoreCase)
            .SelectMany(a => a.Plugins)
            .OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase);

    #endregion
}