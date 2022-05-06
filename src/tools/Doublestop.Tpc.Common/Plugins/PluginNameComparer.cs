using Doublestop.Tpc.Internal;

namespace Doublestop.Tpc.Plugins;

internal sealed class PluginNameComparer : NoCaseTrimmedStringComparer
{
    public static readonly PluginNameComparer Instance = new();
}