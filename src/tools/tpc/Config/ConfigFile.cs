using System.Diagnostics.CodeAnalysis;
using Doublestop.Tpc.Internal;

namespace Doublestop.Tpc.Config;

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
                if (IsCommentOrEmptyLine(line))
                    continue;

                if (!TryParseLine(line, out var key, out var value))
                    continue;

                yield return new KeyValuePair<string, string>(key, value);
            }
        }
    }

    /// <summary>
    /// Returns <c>true</c> if the line is empty, whitespace, or begins with a comment (<c>#</c>).
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    static bool IsCommentOrEmptyLine(string line)
    {
        foreach (var c in line)
            if (c == '#')
                return true;
            else if (!char.IsWhiteSpace(c))
                return false;

        return true;
    }

    /// <summary>
    /// Attempts to parse the line as <c>key = value</c>, trimming whitespace.
    /// </summary>
    /// <remarks>
    /// Ending a line with a comment, eg <c>key = value # set key to value</c>, isn't supported.
    /// In this example, the key setting would be set to <c>value # set key to value</c>.
    /// </remarks>
    /// <param name="line"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
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