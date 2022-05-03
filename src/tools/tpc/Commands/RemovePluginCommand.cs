using System.ComponentModel.DataAnnotations;
using Somethangs.Extensions.CommandLine.Attributes;

namespace Doublestop.Tpc.Commands;

[Command(Name="remove", Description = "Removes an installed plugin.")]
internal sealed class RemovePluginCommand
{
    #region Properties

    [Arg(Description = "Plugin filename, with extension.")]
    [Required]
    public string Plugin { get; init; } = "";

    #endregion
}