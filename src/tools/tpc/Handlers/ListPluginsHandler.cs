using System.Collections.Immutable;
using System.CommandLine.Invocation;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using Doublestop.Tpc.Commands;
using Doublestop.Tpc.Plugins;
using Doublestop.Tpc.Views;
using Somethangs.Extensions.CommandLine;

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

    public override async Task HandleAsync(ListPluginsCommand command, InvocationContext context, CancellationToken cancel)
    {
        var totalPluginCount = await _game.Plugins.CountAsync(cancel);
        var plugins = await GetAllLocalPluginsAsync(command, cancel);

        View pluginsView = command.OutputMode switch
        {
            ListOutputMode.D => new PluginDetailsListView(plugins, false),
            ListOutputMode.A => new PluginDetailsListView(plugins, true),
            ListOutputMode.N => new StringListView(plugins.Select(p => p.Name).Concat(new [] {""})),
            ListOutputMode.F => new StringListView(plugins.Select(p => p.AssemblyFile.Name).Concat(new [] {""})),
        _ => new PluginTableView(plugins)
        };

        new StackLayoutView
        {
            new ContentView(TextSpan.Empty()),
            pluginsView,
            new ContentView($"Matched {plugins.Count}/{totalPluginCount} {(totalPluginCount == 1 ? "plugin" : "plugins")}."),
            new ContentView(TextSpan.Empty())
        }.Render(new ConsoleRenderer(context.Console), Region.Scrolling);
    }

    #endregion

    #region Private Methods

    async ValueTask<IReadOnlyList<InstalledPlugin>> GetAllLocalPluginsAsync(ListPluginsCommand command, CancellationToken cancel)
    {
        var request = SearchPluginsRequest.Empty with
        {
            SearchTerms = command.SearchTerms?.ToImmutableList() ?? ImmutableList<string>.Empty,
            UseRegex = command.UseRegEx,
            MatchAllTerms = command.MatchAllTerms
        };
        var result = await _game.Plugins.SearchAsync(request, cancel);
        return result.Plugins;
    }

    #endregion
}