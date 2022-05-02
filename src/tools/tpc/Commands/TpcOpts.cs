using Somethangs.Extensions.CommandLine.Attributes;

namespace Doublestop.Tpc.Commands;

internal sealed class TpcOpts
{
    [Opt("-c", "--config")]
    public string? ConfigFile { get; init; }

    [Opt("-d", "--game-dir")]
    public string? GameDir { get; init; }
}