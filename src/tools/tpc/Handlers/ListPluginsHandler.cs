using System.Collections.Immutable;
using System.CommandLine.Invocation;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using Doublestop.Tpc.Commands;
using Doublestop.Tpc.Views;
using Doublestop.Extensions.CommandLine;
using Doublestop.Tpc.Internal;
using Doublestop.Tpc.Plugins;
using Doublestop.Tpc.Plugins.Metadata;

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
        var allPlugins = _game.Plugins.AllPlugins.ToImmutableList();
        var matches = SearchPlugins.Instance.Search(
                _game.Plugins,
                command.SearchTerms?.ToImmutableList() ?? ImmutableList<string>.Empty,
                command.UseRegEx,
                command.MatchAllTerms)
            .ToList();

        PrintMatches(matches, allPlugins, command, context);
        return ValueTask.CompletedTask;
    }

    #endregion

    #region Private Methods

    static void PrintMatches(IReadOnlyCollection<PluginMetadata> matches, 
        IReadOnlyCollection<PluginMetadata> allPlugins, 
        ListPluginsCommand command, InvocationContext context)
    {

        context.Console.Render(
            new StackLayoutView
            {
                new ContentView(TextSpan.Empty()),
                GetPluginsView(),
                new ContentView($"Matched {matches.Count}/{allPlugins.Count} {"plugins".Pluralize(allPlugins.Count)}."),
                new ContentView(TextSpan.Empty())
            });

        View GetPluginsView()
        {
            View pluginsView = command.OutputMode switch
            {
                ListOutputMode.D => new PluginDetailsListView(matches, false),
                ListOutputMode.A => new PluginDetailsListView(matches, true),
                ListOutputMode.N => new StringListView(matches.Select(p => p.Name).Concat(new[] { "" })),
                ListOutputMode.F => new StringListView(matches.Select(p => p.AssemblyFileName).Concat(new[] { "" })),
                _ => new PluginTableView(matches)
            };
            return pluginsView;
        }
    }

    #endregion
}