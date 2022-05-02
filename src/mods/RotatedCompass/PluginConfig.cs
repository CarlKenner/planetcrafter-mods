using System;
using System.Diagnostics.CodeAnalysis;
using BepInEx.Configuration;

namespace Doublestop.RotatedCompass
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal sealed class PluginConfig
    {
        #region Fields

        const string ConfigSection = "General";

        const string IAmTheNorthStarDesc =
            "Set to true if you are the courageous sort who follows no one, not even directions.";

        static PluginConfig _instance;

        readonly ConfigEntry<bool> _iAmTheNorthStar;

        #endregion

        #region Constructors

        PluginConfig(ConfigFile config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            _iAmTheNorthStar = config.Bind(ConfigSection, nameof(IAmTheNorthStar), false, IAmTheNorthStarDesc);
        }

        #endregion

        #region Properties

        public static PluginConfig Instance =>
            _instance ??
            throw new InvalidOperationException(
                $"Need to call {nameof(PluginConfig)}.{nameof(Initialize)} first.");

        public bool IAmTheNorthStar => _iAmTheNorthStar.Value;

        #endregion

        #region Public Methods

        internal static void Initialize(ConfigFile config)
        {
            if (_instance != null)
                throw new InvalidOperationException("You already did this. It only needs to be done once.");

            _instance = new PluginConfig(config);
        }

        #endregion
    }
}