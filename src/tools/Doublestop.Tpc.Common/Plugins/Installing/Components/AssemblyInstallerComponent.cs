namespace Doublestop.Tpc.Plugins.Installing.Components;

internal sealed class AssemblyInstallerComponent : InstallerComponent
{
    #region Fields

    readonly string _pluginsDirectory;

    #endregion

    #region Constructors

    public AssemblyInstallerComponent(string pluginsDirectory)
    {
        if (string.IsNullOrWhiteSpace(pluginsDirectory))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(pluginsDirectory));
        _pluginsDirectory = pluginsDirectory;
    }

    #endregion

    #region Public Methods

    public override ValueTask InstallAsync(PluginPackage package, CancellationToken cancel)
    {
        var targetPath = Path.Combine(_pluginsDirectory, package.TargetAssemblyFileName);
        File.Copy(package.SourceAssemblyPath, targetPath, true);
        return ValueTask.CompletedTask;
    }

    public override ValueTask RemoveAsync(string assemblyFileName, CancellationToken cancel)
    {
        var assemblyPath = Path.Combine(_pluginsDirectory, assemblyFileName);
        if (File.Exists(assemblyPath))
            File.Delete(assemblyPath);
        return ValueTask.CompletedTask;
    }

    #endregion
}