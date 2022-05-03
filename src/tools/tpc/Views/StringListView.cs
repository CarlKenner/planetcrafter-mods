using System.CommandLine.Rendering.Views;

namespace Doublestop.Tpc.Views;

internal sealed class StringListView : StackLayoutView
{
    #region Constructors

    public StringListView(IEnumerable<string> values)
        : this(values, Orientation.Vertical)
    {
    }

    public StringListView(IEnumerable<string> values, Orientation orientation) 
        : base(orientation)
    {
        foreach (var value in values)
            Add(new ContentView(value));
    }

    #endregion
}