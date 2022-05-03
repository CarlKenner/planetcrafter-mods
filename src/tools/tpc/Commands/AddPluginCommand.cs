using Doublestop.Extensions.CommandLine.Attributes;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Doublestop.Tpc.Commands;

[Command(Name = "add", Description = "Installs a plugin from a .NET assembly file path, directory, or (eventually) zip archive.")]
internal sealed class AddPluginCommand
{
    #region Properties

    [Arg]
    public string? Plugin { get; init; }

    [Opt("-n", "--filename", Description = "When copying the source assembly, set the destination filename to this value." +
                                           " The .dll extension is not added automatically.")]
    public string? TargetFilename { get; init; }

    #endregion
}