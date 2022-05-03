namespace Doublestop.Tpc;

public sealed class BepInExHelper
{
    #region Fields

    public const string CoreDirectoryName = "core";
    public const string PluginsDirectoryName = "plugins";

    #endregion

    #region Constructors

    public BepInExHelper(string coreDirectory, string pluginsDirectory)
    {
        if (string.IsNullOrWhiteSpace(coreDirectory))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(coreDirectory));
        if (string.IsNullOrWhiteSpace(pluginsDirectory))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(pluginsDirectory));

        CoreDirectory = new DirectoryInfo(coreDirectory);
        PluginsDirectory = new DirectoryInfo(pluginsDirectory);
    }

    #endregion

    #region Properties

    public DirectoryInfo CoreDirectory { get; }
    public DirectoryInfo PluginsDirectory { get; }

    public IEnumerable<FileInfo> CoreDlls =>
        CoreDirectory.EnumerateFiles("*.dll", SearchOption.TopDirectoryOnly);

    #endregion

    #region Public Methods

    public static BepInExHelper CreateDefault(string gameDirectory)
    {
        var bepinexRoot = Path.Combine(gameDirectory, ThePlanetCrafter.BepInExDirectoryName);
        return new BepInExHelper(
            Path.Combine(bepinexRoot, CoreDirectoryName),
            Path.Combine(bepinexRoot, PluginsDirectoryName));
    }

    #endregion
}