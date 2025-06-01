using Exiled.API.Features.Doors;
using HarmonyLib;
using Interactables.Interobjects.DoorUtils;
using ProjectMER.Features.Objects;
using ProjectMER.Features.Serializable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMER.Patches
{
    [HarmonyPatch(typeof(DoorVariant), nameof(DoorVariant.NetworkTargetState), MethodType.Setter)]
    internal static class DoorCloseDisallowPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(DoorVariant __instance, bool value)
        {
            Door door = Door.Get(__instance);
            if (door == null || value != true) return true;
            if (door.GameObject.TryGetComponent<MapEditorObject>(out var merObj) && merObj.Base is SerializableDoor serializableDoor && serializableDoor.IsDisabled == true)
            {
                if (__instance.NetworkTargetState == false) return false;
                else __instance.NetworkTargetState = false;
                return false;
            }
            return true;
        }
    }
}
