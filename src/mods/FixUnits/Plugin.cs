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

namespace CarlKenner.FixUnits
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    internal sealed class Plugin : BaseUnityPlugin
    {
        #region Fields

        const string PluginGuid = "CarlKenner.FixUnits";
        const string PluginName = "CarlKenner.FixUnits";

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
		[HarmonyPatch(typeof(Readable), "GetReadableUnitGeneration")]
		// ReSharper disable InconsistentNaming
		static bool Readable_GetReadableUnitGeneration_Prefix(DataConfig.WorldUnitType _worldUnitType, float _unitValue, ref string __result, ActionnableShowInfos __instance)
		// ReSharper restore InconsistentNaming
		{
			if (_worldUnitType==DataConfig.WorldUnitType.Energy)
				__result = Localization.GetLocalizedString(GameConfig.localizationWorldUnitId + _worldUnitType.ToString()) + " : " + _unitValue.ToString();
			else
				__result = Localization.GetLocalizedString(GameConfig.localizationWorldUnitId + _worldUnitType.ToString()) + " : " + _unitValue.ToString() + " /s";
			return false;
		}

		/// Add thousand separators (probably commas) to displayed values
		[HarmonyPrefix]
		[HarmonyPatch(typeof(WorldUnit), "GetDisplayStringForValue")]
		// ReSharper disable InconsistentNaming
		static bool WorldUnit_GetDisplayStringForValue_Prefix(float _givenValue, bool _roundFinalNum, int _labelIndex, ref string __result, ActionnableShowInfos __instance, List<string> ___unitLabels)
		// ReSharper restore InconsistentNaming
		{
			int num = ((_labelIndex == -1) ? Mathf.Min(Mathf.Max(0, Mathf.FloorToInt(Mathf.Log(_givenValue, 1000f))), ___unitLabels.Count - 1) : _labelIndex);
			double num2 = ((num < 1) ? ((double)_givenValue) : ((double)_givenValue / Math.Pow(1000.0, (double)num)));

			if ((___unitLabels[num] == "ppm") && (num2 >= 1000))
			{
				num2 = num2 / 1000000;
				__result = string.Format("{0:#,0.000%}", num2);
			}
			else
			{
				string text = (_roundFinalNum ? "{0:#,0}" : "{0:#,0.00}");
				__result = string.Format((num < 1) ? text : "{0:#,0.00}", num2) + " " + ___unitLabels[num];
			}
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(WorldUnit), "GetValueString")]
		// ReSharper disable InconsistentNaming
		static bool WorldUnit_GetValueString_Prefix(ref string __result, ActionnableShowInfos __instance, List<string> ___unitLabels, int ___unitLabelIndex, float ___currentTotalValue)
		// ReSharper restore InconsistentNaming
		{
			double num = ((___unitLabelIndex < 1) ? ((double)___currentTotalValue) : ((double)___currentTotalValue / Math.Pow(1000.0, (double)___unitLabelIndex)));
			if ((___unitLabels[___unitLabelIndex] == "ppm") && (num >= 1000))
			{
				num = num / 1000000;
				__result = string.Format("{0:#,0.000%}", num);
			}
			else
			{
				__result = string.Format((___unitLabelIndex < 1) ? "{0:#,0}" : "{0:#,0.00}", num) + " " + ___unitLabels[___unitLabelIndex];
			}
			return false;
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
                    UnityEngine.Object.Instantiate<GameObject>(__instance.groupLineGameObject, __instance.listGridConsumption.transform).GetComponent<UiGroupLine>().SetValues(energyGroupData.group, energyGroupData.number, unit.GetDisplayStringForValue(energyGroupData.generation, false, -1));
                }
            }
            ___energyGroupsData = (from o in ___energyGroupsData
                                   orderby o.generation descending
                                     select o).ToList<EnergyGroupData>();
            foreach (EnergyGroupData energyGroupData2 in ___energyGroupsData)
            {
                if (energyGroupData2.generation >= 0f)
                {
                    UnityEngine.Object.Instantiate<GameObject>(__instance.groupLineGameObject, __instance.listGridProduction.transform).GetComponent<UiGroupLine>().SetValues(energyGroupData2.group, energyGroupData2.number, unit.GetDisplayStringForValue(energyGroupData2.generation, false, -1));
                }
            }
            return false;
        }

		[HarmonyPrefix]
		[HarmonyPatch(typeof(ActionnableShowInfos), "ShowInfos")]
		// ReSharper disable InconsistentNaming
		static bool ActionnableShowInfos_ShowInfos_Prefix(bool _refreshWorldDisplayer, ActionnableShowInfos __instance)
		// ReSharper restore InconsistentNaming
		{
			WorldObjectAssociated componentOnGameObjectOrInParent = Components.GetComponentOnGameObjectOrInParent<WorldObjectAssociated>(__instance.gameObject);
			if (componentOnGameObjectOrInParent == null)
			{
				return false;
			}

			// Call private method __instance.InitIfNeeded();
			MethodInfo methodInfo = typeof(ActionnableShowInfos).GetMethod("InitIfNeeded", BindingFlags.NonPublic | BindingFlags.Instance);
			var parameters = new object[] { };
			methodInfo.Invoke(__instance, parameters);

			WorldObject worldObject = componentOnGameObjectOrInParent.GetWorldObject();
			string text = "";
			if (worldObject != null)
			{
				if (worldObject.GetEnergy() == 0f)
				{
					text = Localization.GetLocalizedString("UI_object_no_energy");
					text += "<br>";
				}
				foreach (WorldUnit worldUnit in Managers.GetManager<WorldUnitsHandler>().GetAllWorldUnits())
				{
					float unitGeneration = worldObject.GetUnitGeneration(worldUnit.GetUnitType());
					if (unitGeneration != 0f)
					{
						text = text + Readable.GetWorldUnitLabel(worldUnit.GetUnitType()) + "  : ";
						if (unitGeneration > 0f)
						{
							text += "+";
						}
						text += worldUnit.GetDisplayStringForValue(unitGeneration, false, -1);
						if (worldUnit.GetUnitType() != DataConfig.WorldUnitType.Energy)
							text += " /s";
						text += "<br>";
					}
				}
				MachineGrower component = componentOnGameObjectOrInParent.gameObject.GetComponent<MachineGrower>();
				if (component != null && component.GetChanceOfGettingSeedsOnHundred() > 0f)
				{
					text = string.Concat(new string[]
					{
						text,
						Localization.GetLocalizedString("UI_chance_to_get_seed"),
						" : ",
						component.GetChanceOfGettingSeedsOnHundred().ToString(),
						"%<br>"
					});
					text = string.Concat(new string[]
					{
						text,
						Localization.GetLocalizedString("UI_growth_speed"),
						" : ",
						(component.GetGrowthSpeed() * 1000f).ToString(),
						"<br>"
					});
					text = string.Concat(new string[]
					{
						text,
						Localization.GetLocalizedString("UI_growth"),
						" : ",
						Mathf.RoundToInt(worldObject.GetGrowth()).ToString(),
						"%<br>"
					});
				}
				if (componentOnGameObjectOrInParent.gameObject.GetComponent<MachineOutsideGrower>() != null)
				{
					text = string.Concat(new string[]
					{
						text,
						Localization.GetLocalizedString("UI_growth"),
						" : ",
						Mathf.RoundToInt(worldObject.GetGrowth()).ToString(),
						"%<br>"
					});
				}
				if (componentOnGameObjectOrInParent.gameObject.GetComponent<MachineGrowerIfLinkedGroup>() != null)
				{
					text = string.Concat(new string[]
					{
						text,
						Localization.GetLocalizedString("UI_growth"),
						" : ",
						Mathf.RoundToInt(worldObject.GetGrowth()).ToString(),
						"%<br>"
					});
				}
			}
			Vector3 position = ((__instance.showPosition != null) ? __instance.showPosition.transform.position : __instance.gameObject.transform.position);
			if (text != null)
			{
				Managers.GetManager<DisplayersHandler>().GetTextWorldDisplayer().ShowTo(text, position, Managers.GetManager<PlayersManager>().GetActivePlayerController().gameObject.GetComponentInChildren<Camera>().gameObject, _refreshWorldDisplayer);
			}
			return false;
		}


		#endregion
	}
}