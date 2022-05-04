using System.Diagnostics;

namespace Doublestop.Tpc.Plugins.Installing;




[DebuggerDisplay("{TargetAssemblyFileName} <= {SourceAssemblyPath}")]
public sealed class PluginPackage
{
    #region Constructors

    public PluginPackage(string sourceAssemblyPath,
        string? targetAssemblyFileName = null, bool includeAssets = true)
    {
        if (string.IsNullOrWhiteSpace(sourceAssemblyPath))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(sourceAssemblyPath));

        SourceAssemblyPath = sourceAssemblyPath;
        IncludeAssets = includeAssets;
        TargetAssemblyFileName = string.IsNullOrWhiteSpace(targetAssemblyFileName)
            ? Path.GetFileName(sourceAssemblyPath)
            : targetAssemblyFileName;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Path to the plugin's .NET assembly.
    /// </summary>
    public string SourceAssemblyPath { get; }

    /// <summary>
    /// Whether to include assets as part of the installation.
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public bool IncludeAssets { get; }

    /// <summary>
    /// Target filename for the plugin's .NET assembly after it is copied to the game's plugins directory.
    /// </summary>
    /// <remarks>
    /// By default, the target filename will retain the same filename as <see cref="SourceAssemblyPath"/>.
    /// </remarks>
    public string TargetAssemblyFileName { get; }

    #endregion

    #region Public Methods

    public static PluginPackage CreateFromFile(string sourceAssembly, string? targetAssemblyName = null)
    {
        return new PluginPackage(sourceAssembly, targetAssemblyName);
    }

    public static PluginPackage CreateFromDirectory(string directory, string? targetAssemblyName = null)
    {
        return CreateFromFile(
            InstallUtil.FindOneAndOnlyAssembly(directory),
            targetAssemblyName);
    }

    #endregion
}