using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace Doublestop.CraftableFabric
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    internal sealed class Plugin : BaseUnityPlugin
    {
        #region Fields

        const string PluginGuid = "Doublestop.CraftableFabric";
        const string PluginName = "Doublestop's Craftable Fabric";
        const string PluginVersion = "0.0.1";
        static ManualLogSource _logger;

        readonly Harmony _harmony = new Harmony(PluginGuid);

        #endregion

        #region Constructors

        public Plugin()
        {
            _logger = Logger;
        }

        #endregion

        #region Properties

        internal new static ManualLogSource Logger => _logger ?? 
                                                      throw new InvalidOperationException("Logger not initialized.");

        #endregion

        #region Private Methods

        void Awake()
        {
            Logger.LogInfo("Plugin loaded. Initializing patch.");
            try
            {
                _harmony.PatchAll(typeof(AddCraftableFabricPatch));
            }
            catch (Exception ex)
            {
                Logger.LogError($"Patch failed to initialize because of an unhandled error: {ex}");
                throw;
            }
        }

        void Destroy()
        {
            _harmony.UnpatchSelf();
        }

        #endregion
    }
}