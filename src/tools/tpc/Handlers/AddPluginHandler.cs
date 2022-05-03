using System.CommandLine.Invocation;
using Doublestop.Tpc.Commands;
using Doublestop.Tpc.Plugins.Installing;
using Somethangs.Extensions.CommandLine;

namespace Doublestop.Tpc.Handlers;

internal sealed class AddPluginHandler : Handler<AddPluginCommand>
{
    #region Fields

    readonly ThePlanetCrafter _game;

    #endregion

    #region Constructors

    public AddPluginHandler(ThePlanetCrafter game)
    {
        _game = game ?? throw new ArgumentNullException(nameof(game));
    }

    #endregion

    #region Public Methods

    public override async Task HandleAsync(AddPluginCommand command, InvocationContext context, CancellationToken cancel)
    {
        var sourcePath = command.Plugin ?? Environment.CurrentDirectory;
        var package = Directory.Exists(sourcePath)
            ? PluginPackage.CreateFromDirectory(sourcePath, command.TargetFilename)
            : PluginPackage.CreateFromFile(sourcePath, command.TargetFilename);
        var plugin = await _game.Plugins.AddAsync(package, cancel);
        context.Console.WriteLine($"Installed {Path.GetFileName(package.SourceAssemblyPath)} to {plugin.AssemblyPath}.");
    }

    #endregion
}