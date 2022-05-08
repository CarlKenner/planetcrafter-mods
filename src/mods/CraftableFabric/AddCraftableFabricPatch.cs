using System;
using System.Collections.Generic;
using HarmonyLib;
using SpaceCraft;

namespace Doublestop.CraftableFabric
{
    public static class AddCraftableFabricPatch
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
                    Plugin.Logger.LogInfo($"Craftable recipe added to fabric (id={CraftableFabricRecipe.FabricBlue}).");
                else
                    Plugin.Logger.LogWarning($"Unable to find fabric item id {CraftableFabricRecipe.FabricBlue}.");
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"Unhandled error updating {CraftableFabricRecipe.FabricBlue}: {ex}");
            }
            return true;
        }
    }
}