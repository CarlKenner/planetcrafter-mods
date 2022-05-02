using Somethangs.Extensions.CommandLine.Attributes;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Thangs.Tpc.Commands;

[Command(Name="install")]
internal sealed class InstallModCommand
{
    [Opt("-xa", "--exclude-assets")]
    public bool ExcludeAssets { get; init; }

    [Arg(MinCount = 0)]
    public string? Mod { get; init; }
}