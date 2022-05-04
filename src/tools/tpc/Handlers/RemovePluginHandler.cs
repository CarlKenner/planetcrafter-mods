using System.CommandLine;
using System.CommandLine.Invocation;
using Doublestop.Tpc.Commands;
using Doublestop.Extensions.CommandLine;
using Doublestop.Tpc.Internal;
using Doublestop.Tpc.Plugins;
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
        var file = await _game.Plugins.GetFileByName(command.File, cancel);
        if (file is null)
        {
            context.SetResult(ErrorCodes.NotFound, $"PluginFile not found: {command.File}");
            return;
        }
        if (!await ValidateAndConfirmAsync(command, file, context, cancel))
            // the validate method sets context result as needed
            return;

        await _game.Plugins.Installer.RemoveAsync(file, cancel);
        context.Console.WriteLine($"Removed {file.Name}.");
    }

    #endregion

    #region Private Methods

    static async ValueTask<bool> ValidateAndConfirmAsync(RemovePluginCommand command, PluginFile file,
        InvocationContext context, CancellationToken cancel)
    {
        var pluginCount = file.Plugins.Count();

        // Zero plugins, it's probably not a plugin file, but some
        // other kind of .dll we don't want to mess around with.
        // erring on the side of caution, report this refusal to the user and exit.
        if (pluginCount == 0)
        {
            context.SetResult(ErrorCodes.NoPluginsInFile,
                $"{file.Name} does not appear to contain any plugins," +
                " and cannot be verified as a valid plugin assembly." +
                " You will have to delete this file manually.");
            return false;
        }

        // One plugin = happy path.
        // Most plugin files will contain a single plugin. No conf required.
        if (pluginCount == 1)
            return true;

        // Multiple plugins. Ask the user to confirm, unless the "no confirm" flag is set in the command.
        if (!command.NoConfirm && !await ConfirmRemoveMultiplePluginsAsync(file, context, cancel))
        {
            context.SetResult(ErrorCodes.NotRemovedBecauseMultiplePlugins, "Cancelled.");
            return false;
        }

        // got confirmation
        return true;
    }

    static async ValueTask<bool> ConfirmRemoveMultiplePluginsAsync(PluginFile file, InvocationContext context, CancellationToken cancel)
    {
        if (Console.IsInputRedirected)
            // tbd: Maybe write a message to stderr like "--no-confirm required when redirecting stdin and deleting a file w/multiple plugins"
            return false;

        // Render the warning/confirmation request
        context.Console.Render(new MultiplePluginsWarningView(file));

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