using System.Diagnostics;

namespace Thangs.Tpc.Mods;

[DebuggerDisplay("{TargetAssemblyFileName} <= {SourceAssemblyPath}")]
public sealed class InstallPackage
{
    #region Constructors

    public InstallPackage(string sourceAssemblyPath,
        string? targetAssemblyFileName = null, string? targetAssetsSubDirName = null, bool includeAssets = true)
    {
        if (string.IsNullOrWhiteSpace(sourceAssemblyPath))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(sourceAssemblyPath));

        SourceAssemblyPath = sourceAssemblyPath;
        IncludeAssets = includeAssets;
        TargetAssemblyFileName = string.IsNullOrWhiteSpace(targetAssemblyFileName)
            ? Path.GetFileName(sourceAssemblyPath)
            : targetAssemblyFileName;
        TargetAssetsSubDirName = string.IsNullOrWhiteSpace(targetAssetsSubDirName)
            ? Path.GetFileNameWithoutExtension(TargetAssemblyFileName)
            : targetAssetsSubDirName;
        if (string.IsNullOrWhiteSpace(TargetAssetsSubDirName))
            TargetAssetsSubDirName = Path.GetFileNameWithoutExtension(TargetAssemblyFileName);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Path to the mod's .NET assembly.
    /// </summary>
    public string SourceAssemblyPath { get; }

    /// <summary>
    /// Whether to include assets as part of the installation.
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public bool IncludeAssets { get; }

    /// <summary>
    /// Target filename for the mod's .NET assembly after it is copied to the game's plugins directory.
    /// </summary>
    /// <remarks>
    /// By default, the target filename will retain the same filename as <see cref="SourceAssemblyPath"/>.
    /// </remarks>
    public string TargetAssemblyFileName { get; }

    /// <summary>
    /// Target name for the mod's assets subdirectory after it is copied to the game's plugins directory.
    /// </summary>
    /// <remarks>
    /// By default, the target asset folder will match the <see cref="TargetAssemblyFileName"/>, without
    /// the extension.
    /// </remarks>
    public string TargetAssetsSubDirName { get; }

    #endregion
}