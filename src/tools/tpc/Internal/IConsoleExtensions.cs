using System.CommandLine;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;

namespace Doublestop.Tpc.Internal;

public static class ConsoleExtensions
{
    public static void Render(this IConsole console, View view, Region? region = null,
        OutputMode outputMode = OutputMode.Auto, bool resetAfterRender = true)
    {
        view.Render(new ConsoleRenderer(console, outputMode, resetAfterRender), region ?? Region.Scrolling);
    }
}