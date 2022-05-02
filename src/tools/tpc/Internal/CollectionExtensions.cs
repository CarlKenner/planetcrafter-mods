namespace Thangs.Tpc.Internal;

internal static class CollectionExtensions
{
    #region Public Methods

    public static IEnumerable<KeyValuePair<TKey, TValue>> DistinctTakeLast<TKey, TValue>(
        this IEnumerable<KeyValuePair<TKey, TValue>> pairs,
        IEqualityComparer<TKey>? keyComparer = null)
    {
        return pairs
            .GroupBy(p => p.Key, keyComparer ?? EqualityComparer<TKey>.Default)
            .Select(g => g.Last());
    }

    #endregion
}