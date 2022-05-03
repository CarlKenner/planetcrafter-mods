using System.CommandLine;
using System.CommandLine.Invocation;
using Doublestop.Tpc.Commands;
using Doublestop.Extensions.CommandLine;

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
        var plugin = await _game.Plugins.GetAsync(command.Plugin, cancel);
        if (plugin is null)
            throw new FileNotFoundException($"Plugin not found: {command.Plugin}");

        await _game.Plugins.RemoveByGuidAsync(plugin.Guid, cancel);
        if (plugin.Exists)
            context.Console.WriteLine($"Removed {plugin.Name}, but the plugin file appears to still exist at {plugin.AssemblyFile.FullName}. It may be locked. Is the game running?");
        else
            context.Console.WriteLine($"Removed {plugin.Name}.");
    }

    #endregion
}