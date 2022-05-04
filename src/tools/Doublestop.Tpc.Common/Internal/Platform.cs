using System.Runtime.InteropServices;

namespace Doublestop.Tpc.Internal;

internal static class Platform
{
    internal static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
}