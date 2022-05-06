using Doublestop.Extensions.CommandLine.Attributes;

namespace Doublestop.Tpc.Commands;

[Command(Name = "remove", Description = "Removes an installed plugin.")]
[Alias("rm")]
internal sealed class RemovePluginCommand
{
    #region Properties

    [Arg(MinCount = 1, Description = "Search pattern matching one or more plugin assemblies to remove. Works the same way as the 'ls' command.")]
    // ReSharper disable once CollectionNeverUpdated.Global
    public List<string>? SearchTerms { get; init; }

    [Opt("-f", "--no-confirm", Description = "Removes plugin assemblies without asking for confirmation." +
                                             " Very dangerous, and a good way to blast away all your plugins by accident." +
                                             " Use with care.")]
    public bool NoConfirm { get; init; }

    [Opt("-e", "--regex", Description = "Search terms are regex patterns. ^ and $ are not implied, meaning regex patterns match substrings by default.")]
    public bool UseRegEx { get; init; }

    [Opt("-a", "--match-all", Description = "By default, a plugin must match one of the search terms to be returned. If this flag is set, a plugin must match all terms.")]
    public bool MatchAllTerms { get; init; }

    #endregion
}