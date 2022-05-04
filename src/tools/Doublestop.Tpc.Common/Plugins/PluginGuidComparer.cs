namespace Doublestop.Tpc.Plugins;

internal sealed class PluginGuidComparer : StringComparer
{
    #region Fields

    public static readonly PluginGuidComparer Instance = new();

    #endregion

    #region Properties

    static StringComparer InnerComparer => OrdinalIgnoreCase;

    #endregion

    #region Public Methods

    public override int Compare(string? x, string? y) => InnerComparer.Compare(x?.Trim(), y?.Trim());

    public override bool Equals(string? x, string? y) => InnerComparer.Equals(x?.Trim(), y?.Trim());

    public override int GetHashCode(string obj) => InnerComparer.GetHashCode(obj.Trim());

    #endregion
}