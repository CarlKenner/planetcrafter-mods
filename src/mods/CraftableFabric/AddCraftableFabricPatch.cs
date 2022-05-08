using System;
using System.Collections.Generic;
using HarmonyLib;
using SpaceCraft;

namespace Doublestop.CraftableFabric
{
    internal static class AddCraftableFabricPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(StaticDataHandler), nameof(StaticDataHandler.LoadStaticData))]
        // ReSharper disable once InconsistentNaming
        static bool StaticDataHandler_LoadStaticData_Prefix(ref List<GroupData> ___groupsData)
        {
            try
            {
                var added = CraftableFabricRecipe.AddCraftableFabric(___groupsData);
                if (added)
                    Plugin.Instance.Logger.LogInfo($"Craftable recipe added to fabric (id={CraftableFabricRecipe.FabricBlue}).");
                else
                    Plugin.Instance.Logger.LogWarning($"Unable to find fabric item id {CraftableFabricRecipe.FabricBlue}.");
            }
            catch (Exception ex)
            {
                Plugin.Instance.Logger.LogError($"Unhandled error updating {CraftableFabricRecipe.FabricBlue}: {ex}");
            }
            return true;
        }
    }
}