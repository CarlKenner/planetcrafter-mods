using System;
using BepInEx;
using HarmonyLib;
using SpaceCraft;
using UnityEngine;

namespace Doublestop.RotatedCompass
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    internal sealed class Plugin : BaseUnityPlugin
    {
        #region Fields

        const string PluginGuid = "Doublestop.RotatedCompass";
        const string PluginName = "Doublestop's Rotated Compass";

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
        [HarmonyPatch(typeof(CanvasCompass), "Update")]
        // ReSharper disable InconsistentNaming
        static void CanvasCompass_Update_Postfix(CanvasCompass __instance, GameObject ___player)
        // ReSharper restore InconsistentNaming
        {
            // Injects a -90 degree y-rotation offset into the original calculation,
            // which shifts the compass labels clockwise one position from default.
            //
            // Looking in the direction of the biggest moon:
            //
            // Game default:
            //
            //          E
            //        N + S
            //          W
            //
            // With this mod enabled:
            //
            //          N
            //        W + E
            //          S
            //
            const int yRotationOffset = -90;
            const int degreesInCircle = 360;
            __instance.compass.uvRect = new Rect(
                (___player.transform.localEulerAngles.y + yRotationOffset) / degreesInCircle,
                __instance.compass.uvRect.y,
                __instance.compass.uvRect.width,
                __instance.compass.uvRect.height);
        }

        #endregion
    }
}