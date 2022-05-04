using System.ComponentModel.DataAnnotations;
using Doublestop.Extensions.CommandLine.Attributes;

namespace Doublestop.Tpc.Commands;

[Command(Name="remove", Description = "Removes an installed plugin.")]
internal sealed class RemovePluginCommand
{
    #region Properties

    [Arg(MinCount = 1,
        Description =
            "The filename of the plugin to remove, including file extension." +
            " Full path is not necessary." +
            " Run the 'ls' command to see a list of filenames for the plugins you have installed.")]
    [Required]
    public string File { get; init; } = "";

    [Opt("-f", "--no-confirm", Description = "Removes the plugin file without asking for confirmation" +
                                             " or checking whether the file contains multiple plugins.")]
    public bool NoConfirm { get; init; }

    #endregion
}