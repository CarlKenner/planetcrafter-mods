namespace Doublestop.Tpc.Mods;

public abstract class InstallStep
{
    #region Public Methods

    public abstract void OnInstall(InstallPackage package, string pluginsDirectory);

    #endregion
}