namespace Thangs.Tpc.Mods;

public sealed class CopyModAssemblyStep : InstallStep
{
    #region Public Methods

    public override void OnInstall(InstallPackage package, string pluginsDirectory)
    {
        var targetPath = Path.Combine(pluginsDirectory, package.TargetAssemblyFileName);
        File.Copy(package.SourceAssemblyPath, targetPath, true);
    }

    #endregion
}