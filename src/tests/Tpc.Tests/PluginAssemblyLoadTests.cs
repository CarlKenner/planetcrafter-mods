using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Doublestop.Tpc;
using Doublestop.Tpc.Internal;
using Doublestop.Tpc.Plugins;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tpc.Tests;

[TestClass]
public sealed class PluginAssemblyLoadTests
{

    [TestMethod]
    public async Task Blah()
    {
        var config = TestUtil.GetTpcConfig();
        var game = new ThePlanetCrafter(config.GameDir!);

        var pluginFile = await game.Plugins.GetFileByName("Doublestop.RotatedCompass.dll", CancellationToken.None);
        pluginFile.Should().NotBeNull();

        using var loadedAssembly = Reflect.LoadAssembly(pluginFile!.Path, game.BepInEx.CoreDlls);
        var plugins = pluginFile.Plugins
            .Select(info => new Plugin(info, pluginFile))
            .ToList();
    }
}