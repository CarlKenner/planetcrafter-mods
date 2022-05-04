using System.Reflection;
using System.Runtime.InteropServices;

namespace Doublestop.Tpc.Internal;

internal static class Reflect
{
    #region Fields

    static readonly Lazy<(string PrivateCoreLib, string MsCorLib)> CoreLibs = new(() =>
    {
        // Tbd: This might need to be made configurable at some point.
        // Hanging it off here so it's easy to get to later.
        var runtimeDirectory = RuntimeEnvironment.GetRuntimeDirectory();
        var systemPrivateCoreLib = Path.Combine(runtimeDirectory, "System.Private.CoreLib.dll");
        var msCorLib = Path.Combine(runtimeDirectory, "mscorlib.dll");
        return (systemPrivateCoreLib, msCorLib);
    });

    #endregion

    #region Public Methods

    internal static LoadedAssembly LoadAssembly(
        string path,
        IEnumerable<string>? dependencies = null)
    {
        var resolver = new PathAssemblyResolver(GetResolverPaths(path, dependencies));
        var context = new MetadataLoadContext(resolver);
        try
        {
            var assembly = context.LoadFromAssemblyPath(path);
            return new LoadedAssembly(assembly, context);
        }
        catch
        {
            context.Dispose();
            throw;
        }
    }

    internal static IEnumerable<string> GetResolverPaths(string pluginAssemblyPath, IEnumerable<string>? bepInExCoreDlls)
    {
        if (string.IsNullOrWhiteSpace(pluginAssemblyPath))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(pluginAssemblyPath));

        // Todo: Allow configuration of alternate/additional resolver paths

        // For now, the following paths are included:
        //   * The assembly being loaded.
        //   * System.Private.CoreLib.dll
        //   * mscorlib.dll
        //   * The Planet Crafter/BepInEx/core/*.dll

        yield return pluginAssemblyPath;
        yield return CoreLibs.Value.PrivateCoreLib;
        yield return CoreLibs.Value.MsCorLib;
        if (bepInExCoreDlls is not null)
            foreach (var file in bepInExCoreDlls)
                yield return file;
    }

    #endregion
}