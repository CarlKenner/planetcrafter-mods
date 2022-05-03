using Somethangs.Extensions.CommandLine.Attributes;

namespace Doublestop.Tpc;

internal sealed class ProgramOpts
{
    #region Properties

    [Opt("-c", "--config")]
    public string? ConfigFile { get; init; }

    [Opt("-d", "--game-dir")]
    public string? GameDir { get; init; }

    #endregion
}