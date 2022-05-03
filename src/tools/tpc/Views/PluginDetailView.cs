using System.Collections.Immutable;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using Doublestop.Tpc.Plugins;

namespace Doublestop.Tpc.Views;

internal sealed class PluginDetailView : StackLayoutView
{
    #region Constructors

    public PluginDetailView(InstalledPlugin plugin, bool showAssets = false)
    {
        if (plugin == null) throw new ArgumentNullException(nameof(plugin));

        AddLine(plugin.Name);
        AddLine(new string('-', plugin.Name.Length));
        AddLine($"File:    {plugin.AssemblyFile.Name}");
        AddLine($"Guid:    {plugin.Guid}");
        AddLine($"Version: {plugin.Version}");
        AddLine("");

        if (showAssets && plugin.AssetsDirectory.Exists)
        {
            var assetFiles = plugin.AssetsDirectory
                .EnumerateFiles("*.*", SearchOption.AllDirectories)
                .OrderBy(f => f.DirectoryName ?? "")
                .Select(f => Path.GetRelativePath(plugin.AssetsDirectory.FullName, f.FullName))
                .ToImmutableList();

            AddLine("");
            AddLine($"{plugin.Name} Assets");
            AddLine(new string('-', plugin.Name.Length) + "-------");
            AddLine($"Directory: {plugin.AssetsDirectory}");
            AddLine($"Files:     {assetFiles.Count}");
            AddLine("");
            foreach (var asset in assetFiles) 
                AddLine(asset);
            AddLine("");
        }
        void AddLine(string text) => Add(new ContentView(text));
    }

    #endregion
}
