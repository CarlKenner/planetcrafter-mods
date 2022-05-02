using System.Collections.ObjectModel;

namespace Doublestop.Tpc.Mods;

public sealed class Installer : Collection<InstallStep>
{
    #region Fields

    readonly string _pluginsDirectory;

    #endregion

    #region Constructors

    public Installer(string pluginsDirectory)
    {
        if (string.IsNullOrWhiteSpace(pluginsDirectory))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(pluginsDirectory));
            
        _pluginsDirectory = pluginsDirectory;

        Add(new CopyModAssemblyStep());
        Add(new CopyModAssetsStep());
    }

    #endregion

    #region Public Methods

    public void Install(InstallPackage package)
    {
        foreach (var step in this)
            step.OnInstall(package, _pluginsDirectory);
    }

    #endregion
}