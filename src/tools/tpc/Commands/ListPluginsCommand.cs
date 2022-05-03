using System.ComponentModel;
using Doublestop.Extensions.CommandLine.Attributes;

namespace Doublestop.Tpc.Commands;

[Command(Name = "ls", Description = "Lists installed plugins.")]
internal sealed class ListPluginsCommand
{
    #region Properties

    [Opt("-e", "--regex", Description = "Search terms are regex patterns. ^ and $ are not implied, meaning regex patterns match substrings by default.")]
    public bool UseRegEx { get; init; }

    [Opt("-o", "--output", Description = "Set the output mode: t=table, n=name, f=filename, d=detailed, a=all.")]
    [DefaultValue(ListOutputMode.T)]
    public ListOutputMode OutputMode { get; init; }

    [Opt("-a", "--match-all", Description = "By default, a plugin must match one of the search terms to be returned. If this flag is set, a plugin must match all terms.")]
    public bool MatchAllTerms { get; init; }

    [Arg(Description = "Optional search terms. Supports * and ? wildcard matching. For substring searches, enclose terms with the appropriate wildcards.")]
    // ReSharper disable once CollectionNeverUpdated.Global
    public List<string>? SearchTerms { get; init; }

    #endregion
}