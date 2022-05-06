using System.Linq;
using Doublestop.Tpc;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tpc.Tests;

[TestClass]
public sealed class PluginAssemblyLoadTests
{

    [TestMethod]
    public void Test()
    {
        var config = TestUtil.GetTpcConfig();
        var game = new ThePlanetCrafter(config.GameDir!);

        var assembly = game.Plugins.GetAssembly("Doublestop.RotatedCompass.dll");
        assembly.Should().NotBeNull();

        var plugins = assembly!.ToList();
        plugins.Count.Should().Be(1);
        plugins[0].Name.Should().Be("Doublestop's Rotated Compass");
    }
}