using System.Collections.Immutable;

namespace Doublestop.Tpc.Plugins;

public sealed record SearchPluginsRequest(
    IReadOnlyCollection<string>? SearchTerms = null,
    bool UseRegex = false,
    bool MatchAllTerms = true)
{
    public const int MaxPageSize = 100;
    public const int DefPageSize = 20;

    public static readonly SearchPluginsRequest Empty = new();

    public IReadOnlyCollection<string> SearchTerms { get; init; } = SearchTerms ?? ImmutableList<string>.Empty;
}