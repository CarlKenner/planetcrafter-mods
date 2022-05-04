using System.Runtime.InteropServices;
using Doublestop.Tpc.Steam;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tpc.Tests
{
    [TestClass]
    public class SteamHelperTests
    {
        [TestMethod]
        public void Steam_Directory_Found_In_Registry()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Assert.Inconclusive("Not windows.");

            var dir = SteamHelper.GetSteamDirectoryFromRegistry();
            dir.Should().NotBeNullOrWhiteSpace();
        }
    }
}