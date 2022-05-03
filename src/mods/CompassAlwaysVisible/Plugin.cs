using System;
using BepInEx;
using HarmonyLib;
using SpaceCraft;
using UnityEngine.UI;

namespace Doublestop.CompassAlwaysVisible
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    internal sealed class Plugin : BaseUnityPlugin
    {
        #region Fields

        const string PluginGuid = "Doublestop.CompassAlwaysVisible";
        const string PluginName = "Doublestop's Compass Always Visible";

        // Make sure the project's <Version/> attr is in sync with PluginVersion
        const string PluginVersion = "0.0.2";

        #endregion

        #region Private Methods

        void Awake()
        {
            Logger.LogInfo("Plugin loaded. Initializing patch.");
            try
            {
                Harmony.CreateAndPatchAll(GetType());
            }
            catch (Exception ex)
            {
                Logger.LogError($"Patch failed to initialize because of an unhandled error: {ex}");
                throw;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CanvasCompass), "SetStatus")]
        // ReSharper disable InconsistentNaming
        static void CanvasCompass_SetStatus_Postfix(RawImage ___compass)
        {
            if (!___compass.gameObject.activeInHierarchy)
                ___compass.gameObject.SetActive(true);
        }
        // ReSharper restore InconsistentNaming

        #endregion
    }
}