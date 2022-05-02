using System.Collections;

namespace Doublestop.Tpc.Config;

internal sealed class CommandLineConfig : IEnumerable<KeyValuePair<string, string>>
{
    public string? GameDir { get; set; }
        
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        if (!string.IsNullOrWhiteSpace(GameDir))
            yield return new KeyValuePair<string, string>(ConfigKeys.GameDir, GameDir);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}