using System.Diagnostics.CodeAnalysis;
using Thangs.Tpc.Internal;

namespace Thangs.Tpc.Config;

internal static class ConfigFile
{
    #region Fields

    const string ConfigDirName = ".tpc";
    const string ConfigFileName = "tpc.cfg";

    #endregion

    #region Properties

    public static DirectoryInfo UserConfigDirectory =>
        new(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ConfigDirName));

    public static FileInfo UserConfigFile =>
        new(Path.Combine(
            UserConfigDirectory.FullName,
            ConfigFileName));

    #endregion

    #region Public Methods

    public static Dictionary<string, string> Read(out bool wasFound)
    {
        return Read(default, out wasFound);
    }

    public static Dictionary<string, string> Read(string? filePath, out bool wasFound)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            filePath = UserConfigFile.FullName;

        var info = new FileInfo(filePath);
        wasFound = info.Exists;
        if (!wasFound)
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        return ReadValuesFromFile(info)
            .DistinctTakeLast(StringComparer.OrdinalIgnoreCase)
            .ToDictionary(p => p.Key, p => p.Value, StringComparer.OrdinalIgnoreCase);
    }

    #endregion

    #region Private Methods

    static IEnumerable<KeyValuePair<string, string>> ReadValuesFromFile(FileInfo file)
    {
        using (var reader = file.OpenText())
        {
            for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
            {
                if (!TryParseLine(line, out var key, out var value))
                    continue;

                yield return new KeyValuePair<string, string>(key, value);
            }
        }
    }

    static bool TryParseLine(string line, [NotNullWhen(true)] out string? key, [NotNullWhen(true)] out string? value)
    {
        const char sep = '=';
        key = null;
        value = null;
        var sepIndex = line.IndexOf(sep);
        if (sepIndex < 0)
            return false;

        key = line[..sepIndex].Trim();
        if (key.Length == 0)
            return false;

        value = sepIndex + 1 < line.Length
            ? line[(sepIndex + 1)..].Trim()
            : "";
        return value.Length != 0;
    }

    #endregion
}