using System.Collections;

namespace Doublestop.Tpc.Internal;

internal class NoCaseTrimmedStringComparer : IComparer, IEqualityComparer, IComparer<string?>, IEqualityComparer<string?>
{
    #region Properties

    public static readonly NoCaseTrimmedStringComparer Default = new();

    static StringComparer InnerComparer => StringComparer.OrdinalIgnoreCase;

    #endregion

    #region Public Methods

    public int Compare(string? x, string? y) => InnerComparer.Compare(x?.Trim(), y?.Trim());

    public bool Equals(string? x, string? y) => InnerComparer.Equals(x?.Trim(), y?.Trim());

    public int GetHashCode(string obj) => InnerComparer.GetHashCode(obj.Trim());

    #endregion

    #region Private Methods

    int IComparer.Compare(object? x, object? y) => 
        Compare(x as string, y as string);

    bool IEqualityComparer.Equals(object? x, object? y) => 
        Equals(x as string, y as string);

    int IEqualityComparer.GetHashCode(object obj) => 
        obj is string s ? GetHashCode(s) : 0;

    #endregion
}