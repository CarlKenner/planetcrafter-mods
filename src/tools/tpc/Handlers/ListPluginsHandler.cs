using System.CommandLine.Invocation;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using System.Text.RegularExpressions;
using Doublestop.Tpc.Commands;
using Doublestop.Tpc.Views;
using Doublestop.Extensions.CommandLine;
using Doublestop.Tpc.Internal;

namespace Doublestop.Tpc.Handlers;

internal sealed class ListPluginsHandler : Handler<ListPluginsCommand>
{
    #region Fields

    readonly ThePlanetCrafter _game;

    #endregion

    #region Constructors

    public ListPluginsHandler(ThePlanetCrafter game)
    {
        _game = game ?? throw new ArgumentNullException(nameof(game));
    }

    #endregion

    #region Public Methods

    public override ValueTask HandleAsync(ListPluginsCommand command, InvocationContext context, CancellationToken cancel)
    {
        var allPlugins = _game.Plugins.Plugins.ToList();
        var termsPredicate = CreateTermsPredicate(command.SearchTerms, command.UseRegEx, command.MatchAllTerms);
        var matches = allPlugins
            .Where(p => termsPredicate(p.Name) ||
                        termsPredicate(p.Guid) ||
                        termsPredicate(p.File.Name))
            .Distinct()
            .ToList();

        View pluginsView = command.OutputMode switch
        {
            ListOutputMode.D => new PluginDetailsListView(matches, false),
            ListOutputMode.A => new PluginDetailsListView(matches, true),
            ListOutputMode.N => new StringListView(matches.Select(p => p.Name).Concat(new[] { "" })),
            ListOutputMode.F => new StringListView(matches.Select(p => p.File.Name).Concat(new[] { "" })),
            _ => new PluginTableView(matches)
        };

        context.Console.Render(new StackLayoutView
        {
            new ContentView(TextSpan.Empty()),
            pluginsView,
            new ContentView($"Matched {matches.Count}/{allPlugins.Count} {"plugins".Pluralize(allPlugins.Count)}."),
            new ContentView(TextSpan.Empty())
        });

        return ValueTask.CompletedTask;
    }

    #endregion

    #region Private Methods

    static Func<string?, bool> CreateTermsPredicate(IEnumerable<string>? searchTerms, bool useRegex, bool matchAll)
    {
        if (searchTerms is null)
            return _ => true;

        var termList = searchTerms.Distinct().ToList();
        if (!termList.Any())
            return _ => true;

        if (useRegex)
        {
            var regexList = termList.Select(s => new Regex(s, RegexOptions.IgnoreCase)).ToArray();
            if (matchAll)
                return s => regexList.All(re => re.IsMatch(s ?? ""));

            return s => regexList.Any(re => re.IsMatch(s ?? ""));
        }
        var matcher = new StringPatternMatcher(true);
        if (matchAll)
            return s => termList.WhereNotNull().All(term => matcher.IsMatch(s ?? "", term));
        
        return s => termList.WhereNotNull().Any(term => matcher.IsMatch(s ?? "", term));
    }

    #endregion
}