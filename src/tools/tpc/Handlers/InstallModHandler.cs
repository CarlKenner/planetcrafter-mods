using System.CommandLine.Invocation;
using Somethangs.Extensions.CommandLine;
using Thangs.Tpc.Commands;
using Thangs.Tpc.Mods;

namespace Thangs.Tpc.Handlers;

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
                ? InstallUtil.FindFirstAndOnlyAssemblySourcePath(sourcePath)
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