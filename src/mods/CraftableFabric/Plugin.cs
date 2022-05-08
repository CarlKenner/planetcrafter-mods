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
        static Plugin _instance;

        #endregion

        #region Constructors

        public Plugin()
        {
            _instance = this;
        }

        #endregion

        #region Properties


        internal new ManualLogSource Logger => base.Logger;
        internal static Plugin Instance => _instance ?? throw new InvalidOperationException("Not initialized.");

        #endregion

        #region Private Methods

        void Awake()
        {
            Logger.LogInfo("Plugin loaded. Initializing patch.");
            try
            {
                Harmony.CreateAndPatchAll(typeof(AddCraftableFabricPatch));
            }
            catch (Exception ex)
            {
                Logger.LogError($"Patch failed to initialize because of an unhandled error: {ex}");
                throw;
            }
        }

        #endregion
    }
}