using Doublestop.Tpc.Config;

namespace Tpc.Tests;

internal static class TestUtil
{
    internal static TpcConfig GetTpcConfig() =>
        new ConfigBuilder()
            .AddDefaults()
            .AddConfigFile(null)
            .Build();
}