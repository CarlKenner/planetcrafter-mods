namespace Doublestop.Tpc.Mods;

public class RemoveAssemblyFileStep : RemoveStep
{
    #region Fields

    readonly string _pluginsDirectory;

    #endregion

    #region Constructors

    public RemoveAssemblyFileStep(string pluginsDirectory)
    {
        if (string.IsNullOrWhiteSpace(pluginsDirectory))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(pluginsDirectory));

        _pluginsDirectory = pluginsDirectory;
    }

    #endregion

    #region Public Methods

    public override void OnRemove(InstallPackage package)
    {
        var assemblyPath = Path.Combine(_pluginsDirectory, package.TargetAssemblyFileName);
        if (File.Exists(assemblyPath))
            File.Delete(assemblyPath);
    }

    #endregion
}