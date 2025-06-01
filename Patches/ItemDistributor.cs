using HarmonyLib;
using MapGeneration.Distributors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMER.Patches
{
    [HarmonyPatch(typeof(ItemDistributor), nameof(ItemDistributor.ServerCreatePickup))]
    internal class ItemDistributorPatch
    {
        private static bool Prefix() { return !ProjectMER.Singleton.Config.DisableItemDistributor; }
    }
}
