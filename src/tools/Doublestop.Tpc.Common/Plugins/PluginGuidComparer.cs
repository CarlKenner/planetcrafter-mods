using Doublestop.Tpc.Plugins.Metadata;

namespace Doublestop.Tpc.Internal;

internal sealed class PluginGuidComparer : EqualityComparer<PluginGuid>
{
    public static readonly PluginGuidComparer Instance = new();

    public override bool Equals(PluginGuid x, PluginGuid y) => 
        NoCaseTrimmedStringComparer.Default.Equals(x.Value, y.Value);

    public override int GetHashCode(PluginGuid obj) => 
        NoCaseTrimmedStringComparer.Default.GetHashCode(obj.Value);
}