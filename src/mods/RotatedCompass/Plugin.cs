using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using HarmonyLib;
using SpaceCraft;
using MijuTools;
using UnityEngine;
using Unity;
using UnityEngine.UI;

namespace Doublestop.RotatedCompass
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    internal sealed class Plugin : BaseUnityPlugin
    {
        #region Fields

        const string PluginGuid = "CarlKenner.FixUnits";
        const string PluginName = "CarlKenner.FixUnits";

        // Make sure the project's <Version/> attr is in sync with PluginVersion
        const string PluginVersion = "0.0.1";

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
        [HarmonyPatch(typeof(UiWindowEnergy), "DisplayValues")]
        // ReSharper disable InconsistentNaming
        static void UiWindowEnergy_DisplayValues_Postfix(UiWindowEnergy __instance)
        // ReSharper restore InconsistentNaming
        {
            __instance.increaseValue.text = __instance.increaseValue.text.Replace("/h", "");
            __instance.decreaseValue.text = __instance.decreaseValue.text.Replace("/h", "");
            __instance.generationValue.text = __instance.generationValue.text.Replace("/h", "");
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UiWindowEnergy), "ListGroupsEnergy")]
        // ReSharper disable InconsistentNaming
        static bool UiWindowEnergy_ListGroupsEnergy_Prefix(UiWindowEnergy __instance, ref List<EnergyGroupData> ___energyGroupsData)
        // ReSharper restore InconsistentNaming
        {
            GameObjects.DestroyAllChildren(__instance.listGridConsumption.gameObject, false);
            GameObjects.DestroyAllChildren(__instance.listGridProduction.gameObject, false);
            WorldUnit unit = Managers.GetManager<WorldUnitsHandler>().GetUnit(DataConfig.WorldUnitType.Energy);
            ___energyGroupsData = (from o in ___energyGroupsData
                                           orderby o.generation
                                     select o).ToList<EnergyGroupData>();
            foreach (EnergyGroupData energyGroupData in ___energyGroupsData)
            {
                if (energyGroupData.generation <= 0f)
                {
                    UnityEngine.Object.Instantiate<GameObject>(__instance.groupLineGameObject, __instance.listGridConsumption.transform).GetComponent<EnergyGroupLine>().SetValues(energyGroupData.group, energyGroupData.number, unit.GetDisplayStringForValue(energyGroupData.generation, false, -1));
                }
            }
            ___energyGroupsData = (from o in ___energyGroupsData
                                   orderby o.generation descending
                                     select o).ToList<EnergyGroupData>();
            foreach (EnergyGroupData energyGroupData2 in ___energyGroupsData)
            {
                if (energyGroupData2.generation >= 0f)
                {
                    UnityEngine.Object.Instantiate<GameObject>(__instance.groupLineGameObject, __instance.listGridProduction.transform).GetComponent<EnergyGroupLine>().SetValues(energyGroupData2.group, energyGroupData2.number, unit.GetDisplayStringForValue(energyGroupData2.generation, false, -1));
                }
            }
            return false;
        }
        #endregion
    }
}