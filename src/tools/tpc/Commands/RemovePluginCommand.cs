using System.ComponentModel.DataAnnotations;
using Doublestop.Extensions.CommandLine.Attributes;

namespace Doublestop.Tpc.Commands;

[Command(Name="remove", Description = "Removes an installed plugin.")]
internal sealed class RemovePluginCommand
{
    #region Properties

    [Arg(Description = "Plugin guid. Run the 'ls' command to see a list of guids for the plugins you have installed.")]
    [Required]
    public string Plugin { get; init; } = "";

    #endregion
}