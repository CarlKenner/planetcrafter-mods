﻿using System;
using System.Diagnostics.CodeAnalysis;
using BepInEx;
using HarmonyLib;
using SpaceCraft;
using UnityEngine.UI;

namespace Doublestop.CompassAlwaysVisible
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public sealed class Plugin : BaseUnityPlugin
    {
        #region Fields

        public const string PluginGuid = "Doublestop.CompassAlwaysVisible";
        public const string PluginName = "Doublestop's Compass Always Visible";

        // Make sure the project's <Version/> attr is in sync with PluginVersion
        public const string PluginVersion = "0.0.1";

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
        [HarmonyPatch(typeof(CanvasCompass), "SetActive")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        static void CanvasCompass_SetActive_Postfix(RawImage ___compass)
        {
            if (!___compass.gameObject.activeInHierarchy)
                ___compass.gameObject.SetActive(true);
        }

        #endregion
    }
}