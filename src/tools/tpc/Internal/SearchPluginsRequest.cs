using System.Collections.Immutable;

namespace Doublestop.Tpc.Internal;

public sealed record SearchPluginsRequest(
    IReadOnlyCollection<string>? SearchTerms = null,
    bool UseRegex = false,
    bool MatchAllTerms = true)
{
    public static readonly SearchPluginsRequest Empty = new();

    public IReadOnlyCollection<string> SearchTerms { get; init; } = SearchTerms ?? ImmutableList<string>.Empty;
}