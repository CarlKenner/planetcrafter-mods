using System.CommandLine;
using System.CommandLine.Invocation;
using Doublestop.Tpc.Commands;
using Doublestop.Tpc.Plugins.Installing;
using Doublestop.Extensions.CommandLine;

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

    public override async ValueTask HandleAsync(AddPluginCommand command, InvocationContext context, CancellationToken cancel)
    {
        var package = CreateInstallPackage(command);
        var pluginFile = await _game.InstallPluginAssembly(package, cancel);
        if (!pluginFile.Exists)
            throw new FileNotFoundException($"Plugin file {pluginFile.Path} was not found after installation.");

        context.Console.WriteLine($"Installed {Path.GetFileName(package.SourceAssemblyPath)} to {pluginFile.Path}.");
    }

    static PluginPackage CreateInstallPackage(AddPluginCommand command)
    {
        // Todo: Support for zip files and http download.
        // Will have to break this out into some simple classes that know how
        // to fetch their respective resources (plugin file, assets)
        // and return them to the installer.
        // Which means dealing with streams, probably.
        // In turn, that means File.Copy isn't going to cut it anymore.
        var sourcePath = command.Plugin ?? Environment.CurrentDirectory;
        var package = Directory.Exists(sourcePath)
            ? PluginPackage.CreateFromDirectory(sourcePath, command.TargetFilename)
            : PluginPackage.CreateFromFile(sourcePath, command.TargetFilename);
        return package;
    }

    #endregion
}