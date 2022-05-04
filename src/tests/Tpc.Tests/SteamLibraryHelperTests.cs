using Doublestop.Tpc;
using Doublestop.Tpc.Steam;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tpc.Tests;

[TestClass]
public class SteamLibraryHelperTests
{
    [TestMethod]
    public void LocateGameDirectoryFromSteamLibrary()
    {
        var steam = new SteamHelper();
        steam.SteamDirectory.Exists.Should().BeTrue();

        var gameDir = steam.Library.GetGameDirectory(ThePlanetCrafter.AppId);
        gameDir.Should().NotBeNull();
    }
}