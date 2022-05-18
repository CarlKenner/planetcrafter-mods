using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using HarmonyLib;
using SpaceCraft;
using MijuTools;
using UnityEngine;
using Unity;
using UnityEngine.UI;
using System.Reflection;

namespace CarlKenner.FixShallowWater
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    internal sealed class Plugin : BaseUnityPlugin
    {
        #region Fields

        const string PluginGuid = "CarlKenner.FixShallowWater";
        const string PluginName = "CarlKenner.FixShallowWater";

        // Make sure the project's <Version/> attr is in sync with PluginVersion
        const string PluginVersion = "0.0.1";

		static Collider lastNonWaterCollider = null;

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

		[HarmonyPrefix]
		[HarmonyPatch(typeof(PlayerAimController), "HandleAiming")]
		// ReSharper disable InconsistentNaming
		static bool PlayerAimController_HandleAiming_Prefix(ActionnableShowInfos __instance, PlayerMultitool ___playerMultitool, Ray ___aimingRay, ref float ___distanceHitLimit, 
			                                                ref int ___layerMask, ref DataConfig.MultiToolState ___previousMultitoolState, ref Collider ___lastAimedCollider, 
															ref List<Actionnable> ___lastAimedActionnables, ref List<Actionnable> ___aimedActionnables)
		// ReSharper restore InconsistentNaming
		{
			// Don't change Deconstruction
			if (___playerMultitool.GetState() == DataConfig.MultiToolState.Deconstruct)
				return true;

			// If they're thirsty (warning, low hydration) then always return the water, even if there's something under it
			// Once they click drink and are no longer in warning territory, they'll be able to target the thing under the water
			if (__instance.GetComponent<PlayerGaugesHandler>().GetPlayerGaugeThirst().GetValue() <= 25)
				return true;

			// Do the same thing SetLayerMaskAndDistance(), we can't call it directly because it is private
			___layerMask = ~LayerMask.GetMask(GameConfig.commonIgnoredLayers);
			___distanceHitLimit = 3.5f;

			int waterMask = LayerMask.GetMask(new string[] { GameConfig.layerWaterName });
			int ignoreWaterMask = ___layerMask & ~waterMask;

			RaycastHit raycastHit;
			if (Physics.Raycast(___aimingRay, out raycastHit, ___distanceHitLimit, ignoreWaterMask))
			{
				// initialization
				if (___previousMultitoolState != ___playerMultitool.GetState())
				{
					___lastAimedCollider = null;
					lastNonWaterCollider = null;
				}
				___previousMultitoolState = ___playerMultitool.GetState();

				// if we hit the same actionable as last time, we have finished and don't need to do anything
				// (water may have gotten in the way since last time, but we'll ignore it until we stop aiming at the same thing)
				if (raycastHit.collider == ___lastAimedCollider && ___lastAimedActionnables.Count() > 0)
				{
					return false;
				}
				lastNonWaterCollider = raycastHit.collider;
				List<Actionnable> list = new List<Actionnable>(raycastHit.transform.GetComponents<Actionnable>());
				if (list.Count == 0)
				{
					// we didn't hit any actionables, so try again with the original function that includes water
					return true;
				}
				if (list.Count > 0)
				{
					// we hit something actionable while ignoring water, so let's report it's action
					___lastAimedCollider = lastNonWaterCollider;

					// Call private method __instance.SetHoverOut(___lastAimedActionnables);
					MethodInfo methodInfo = typeof(PlayerAimController).GetMethod("SetHoverOut", BindingFlags.NonPublic | BindingFlags.Instance);
					var parameters = new object[] { ___lastAimedActionnables };
					methodInfo.Invoke(__instance, parameters);

					// Call private method __instance.SetOnHover(list);
					methodInfo = typeof(PlayerAimController).GetMethod("SetOnHover", BindingFlags.NonPublic | BindingFlags.Instance);
					parameters = new object[] { list };
					methodInfo.Invoke(__instance, parameters);

					___aimedActionnables = list;
					___lastAimedActionnables = list;

					// we're done here, no need to call original function
					return false;
				}
				return true;
			}
			else
			{
				// We didn't hit anything while ignoring water, so try again with the original function to see if we hit water
				return true;
			}
		}
		#endregion
	}
}