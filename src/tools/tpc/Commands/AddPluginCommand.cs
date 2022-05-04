using Doublestop.Extensions.CommandLine.Attributes;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Doublestop.Tpc.Commands;

[Command(Name = "add", Description = Constants.CommandDesc)]
internal sealed class AddPluginCommand
{
    #region Properties

    [Arg]
    public string? Plugin { get; init; }

    [Opt("-f", "--force", Description = Constants.ForceDesc)]
    public bool Force { get; init; }

    [Opt("-n", "--filename", Description = Constants.TargetFilenameDesc)]
    public string? TargetFilename { get; init; }

    #endregion

    #region Nested Types

    static class Constants
    {
        #region Fields

        internal const string CommandDesc = "Installs a plugin from a .NET assembly file path, directory, or (eventually) zip archive.";
        internal const string ForceDesc = "Force install over a pre-existing version, even if that version is newer. This flag is required to roll a plugin back to an older version.";

        internal const string TargetFilenameDesc = "When copying the source assembly, set the destination filename to this value." +
                                                   " The .dll extension is not added automatically.";

        #endregion
    }

    #endregion
}