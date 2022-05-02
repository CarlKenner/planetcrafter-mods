using System.CommandLine.Invocation;
using Doublestop.Tpc.Commands;
using Doublestop.Tpc.Mods;
using Somethangs.Extensions.CommandLine;

namespace Doublestop.Tpc.Handlers;

// ReSharper disable once UnusedType.Global
internal sealed class InstallModHandler : Handler<InstallModCommand>
{
    readonly PlanetCrafterGame _game;

    public InstallModHandler(PlanetCrafterGame game)
    {
        _game = game ?? throw new ArgumentNullException(nameof(game));
    }

    public override async Task HandleAsync(InstallModCommand command, InvocationContext context, CancellationToken cancel)
    {
        await Task.Run(() =>
        {
            var sourcePath = command.Mod ?? Environment.CurrentDirectory;
            var assemblyFile = Directory.Exists(sourcePath)
                ? InstallUtil.FindOneAndOnlyAssembly(sourcePath)
                : Path.GetFullPath(sourcePath);
            var settings = new InstallPackage(assemblyFile,
                Path.GetFileName(assemblyFile),
                Path.GetFileNameWithoutExtension(assemblyFile));
            // todo: Make everything async again, heh.
            _game.InstallMod(settings);
            return Task.FromResult(0);
        }, cancel);
    }
}