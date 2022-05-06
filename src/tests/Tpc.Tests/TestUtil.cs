using Doublestop.Tpc;
using Doublestop.Tpc.Config;

namespace Tpc.Tests;

internal static class TestUtil
{
    internal static TpcConfig GetTpcConfig() =>
        new ConfigBuilder()
            .AddDefaults()
            .AddConfigFile(null)
            .Build();

    internal static ThePlanetCrafter CreateGameInstance(string? gameDir = null) => 
        new(GetTpcConfig().GameDir);
}