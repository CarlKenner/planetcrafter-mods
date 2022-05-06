using System.CommandLine.Rendering.Views;
using Doublestop.Tpc.Plugins.Metadata;

namespace Doublestop.Tpc.Views;

internal sealed class PluginDetailView : StackLayoutView
{
    #region Constructors

    public PluginDetailView(PluginMetadata plugin, bool showAssets = false)
    {
        if (plugin == null) throw new ArgumentNullException(nameof(plugin));

        AddLine(plugin.Name);
        AddLine(new string('-', plugin.Name.Length));
        AddLine($"File:    {plugin.AssemblyFileName}");
        AddLine($"Guid:    {plugin.Guid}");
        AddLine($"Version: {plugin.Version}");
        AddLine($"Dependencies: {string.Join(", ", plugin.Dependencies.Select(d => d.Guid))}");
        AddLine("");
        //if (showAssets && plugin.Equals())
        //{
        //    var assetFiles = plugin.AssetsDirectory
        //        .EnumerateFiles("*.*", SearchOption.AllDirectories)
        //        .OrderBy(f => f.DirectoryName ?? "")
        //        .Select(f => Path.GetRelativePath(plugin.AssetsDirectory.FullName, f.FullName))
        //        .ToImmutableList();

        //    AddLine("");
        //    AddLine($"{plugin.Name} Assets");
        //    AddLine(new string('-', plugin.Name.Length) + "-------");
        //    AddLine($"Directory: {plugin.AssetsDirectory}");
        //    AddLine($"Files:     {assetFiles.Count}");
        //    AddLine("");
        //    foreach (var asset in assetFiles) 
        //        AddLine(asset);
        //    AddLine("");
        //}
        void AddLine(string text) => Add(new ContentView(text));
    }

    #endregion
}
