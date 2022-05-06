using System.Reflection;
using System.Runtime.InteropServices;

namespace Doublestop.Tpc.Internal;

internal sealed class LoadedAssembly : IDisposable
{
    #region Constructors

    public LoadedAssembly(string assemblyPath, MetadataLoadContext context) 
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        Assembly = Context.LoadFromAssemblyPath(assemblyPath);
    }

    #endregion

    #region Properties

    /// <summary>
    /// The loaded assembly.
    /// </summary>
    public Assembly Assembly { get; init; }

    /// <summary>
    /// The context into which <see cref="Assembly"/> was loaded.
    /// </summary>
    public MetadataLoadContext Context { get; init; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Disposes the underlying <see cref="Context"/>.
    /// Behavior of this instance is undefined after this method has been called.
    /// </summary>
    public void Dispose() => Context.Dispose();

    /// <summary>
    /// Loads a plugin assembly from file.
    /// </summary>
    /// <param name="assemblyPath">Full path to the plugin assembly.</param>
    /// <param name="dependencies">Collection of dependencies, used to initialize a <see cref="PathAssemblyResolver"/>.</param>
    /// <param name="includeSystemCoreLibs">Whether to include <c>System.Private.CoreLib.dll</c>
    /// and <c>mscorlib.dll</c> with <see cref="dependencies"/>.
    /// </param>
    /// <returns></returns>
    public static LoadedAssembly Load(string assemblyPath, IEnumerable<string> dependencies, bool includeSystemCoreLibs = true)
    {
        var context = new MetadataLoadContext(new PathAssemblyResolver(GetResolverPaths()));
        try
        {
            return new LoadedAssembly(assemblyPath, context);
        }
        catch
        {
            context.Dispose();
            throw;
        }

        IEnumerable<string> GetResolverPaths()
        {
            // order: assembly, deps, sys core libs

            yield return assemblyPath;
            foreach (var path in dependencies)
                yield return path;

            if (!includeSystemCoreLibs)
                yield break;

            const string sysPrivateCoreLib = "System.Private.CoreLib.dll";
            const string mscorLib = "mscorlib.dll";

            var runtimeDir = RuntimeEnvironment.GetRuntimeDirectory();
            yield return Path.Combine(runtimeDir, sysPrivateCoreLib);
            yield return Path.Combine(runtimeDir, mscorLib);
        }
    }

    #endregion
}