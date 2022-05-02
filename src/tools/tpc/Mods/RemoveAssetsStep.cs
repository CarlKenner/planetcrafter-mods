namespace Thangs.Tpc.Mods;

public class RemoveAssetsStep : RemoveStep
{
    #region Fields

    readonly string _pluginsDirectory;

    #endregion

    #region Constructors

    public RemoveAssetsStep(string pluginsDirectory)
    {
        if (string.IsNullOrWhiteSpace(pluginsDirectory))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(pluginsDirectory));

        _pluginsDirectory = pluginsDirectory;
    }

    #endregion

    #region Public Methods

    public override void OnRemove(InstallPackage package)
    {
        var assetsDir = Path.Combine(_pluginsDirectory, package.TargetAssetsSubDirName);
        if (Directory.Exists(assetsDir))
            Directory.Delete(assetsDir, true);
    }

    #endregion
}