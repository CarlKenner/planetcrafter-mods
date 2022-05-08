using System;
using System.Collections.Generic;
using System.Linq;
using SpaceCraft;

namespace Doublestop.CraftableFabric
{
    internal static class CraftableFabricRecipe
    {
        #region Fields

        internal const string FabricBlue = "FabricBlue";
        const string BioPlastic1 = "Bioplastic1";
        const string VegetableBeans = "Vegetable2Growable";
        const string Magnesium = "Magnesium";
        const string WaterBottle1 = "WaterBottle1";

        #endregion

        #region Public Methods

        public static bool AddCraftableFabric(List<GroupData> groupsData)
        {
            var fabric = GetItem(groupsData, FabricBlue);
            if (fabric is null)
                return false;

            fabric.craftableInList = new List<DataConfig.CraftableIn> { DataConfig.CraftableIn.CraftBioLab };
            fabric.unlockingWorldUnit = DataConfig.WorldUnitType.Terraformation;
            fabric.unlockingValue = 0;
            fabric.recipeIngredients = GetIngredients(groupsData).ToList();
            return true;
        }

        #endregion

        #region Private Methods

        static GroupDataItem GetItem(IEnumerable<GroupData> groups, string id) =>
            groups.OfType<GroupDataItem>().FirstOrDefault(t => t.id == id);

        static IEnumerable<GroupDataItem> GetIngredients(IReadOnlyCollection<GroupData> groups)
        {
            yield return GetItemOrDie(groups, BioPlastic1);
            yield return GetItemOrDie(groups, VegetableBeans);
            yield return GetItemOrDie(groups, Magnesium);
            yield return GetItemOrDie(groups, WaterBottle1);
        }

        static GroupDataItem GetItemOrDie(IEnumerable<GroupData> groups, string id) =>
            GetItem(groups, id) ?? throw ItemNotFound(id);

        static Exception ItemNotFound(string id) => new Exception($"Item not found: {id}");

        #endregion
    }
}