using System.Collections.Immutable;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using Doublestop.Tpc.Commands;
using Doublestop.Extensions.CommandLine;
using Doublestop.Tpc.Internal;
using Doublestop.Tpc.Plugins;
using Doublestop.Tpc.Plugins.Metadata;
using Doublestop.Tpc.Views;

namespace Doublestop.Tpc.Handlers;

internal sealed class RemovePluginHandler : Handler<RemovePluginCommand>
{
    #region Fields

    readonly ThePlanetCrafter _game;

    #endregion

    #region Constructors

    public RemovePluginHandler(ThePlanetCrafter game)
    {
        _game = game ?? throw new ArgumentNullException(nameof(game));
    }

    #endregion

    #region Public Methods

    public override async ValueTask HandleAsync(RemovePluginCommand command, InvocationContext context, CancellationToken cancel)
    {
        var plugins = SearchPlugins.Instance.Search(_game.Plugins,
                command.SearchTerms?.ToImmutableList() ?? ImmutableList<string>.Empty,
                false,
                true)
            .ToImmutableList();

        if (!plugins.Any())
        {
            context.SetResult(ErrorCodes.NotFound, "Found no matching plugins.");
            return;
        }
        if (!await ValidateAndConfirmAsync(command, plugins, context, cancel))
            // the validate method sets context result as needed
            return;

        var pluginAssemblies = plugins
            .Select(p => p.AssemblyFileName)
            .Distinct(NoCaseTrimmedStringComparer.Default);

        foreach (var assemblyFile in pluginAssemblies)
        {
            var assembly = _game.Plugins.GetAssembly(assemblyFile);
            if (assembly is null)
                continue;

            await _game.RemovePluginAssembly(assembly, cancel);
            context.Console.WriteLine($"Removed {assembly.Name}.");
        }
    }

    #endregion

    #region Private Methods

    static async ValueTask<bool> ValidateAndConfirmAsync(RemovePluginCommand command, IReadOnlyList<PluginMetadata> plugins,
        InvocationContext context, CancellationToken cancel)
    {
        var pluginCount = plugins.Count;
        Debug.Assert(pluginCount > 0, "pluginCount > 0");
        if (!command.NoConfirm && !await ConfirmRemoveAsync(plugins, context, cancel))
        {
            context.SetResult(ErrorCodes.NotRemovedBecauseMultiplePlugins, "Cancelled.");
            return false;
        }
        // got confirmation
        return true;
    }

    static async ValueTask<bool> ConfirmRemoveAsync(IReadOnlyList<PluginMetadata> plugins, InvocationContext context, CancellationToken cancel)
    {
        if (Console.IsInputRedirected)
            // tbd: Maybe write a message to stderr like "--no-confirm required when redirecting stdin and deleting a file w/multiple plugins"
            return false;

        // Render the warning/confirmation request
        context.Console.Render(new MultiplePluginsWarningView(plugins));

        // Get the user's yes/no response.
        var confirmed = await ConsoleUtil.WaitForConfirmationAsync(true, 100, cancel);
        // Cursor is still at the end of the confirmation message, so reset position with a newline.
        context.Console.WriteLine("");
        return confirmed;
    }

    #endregion

    #region Nested Types

    static class ErrorCodes
    {
        #region Fields

        internal const int NotFound = 2;
        internal const int NoPluginsInFile = 3;
        internal const int NotRemovedBecauseMultiplePlugins = 4;

        #endregion
    }

    #endregion
}