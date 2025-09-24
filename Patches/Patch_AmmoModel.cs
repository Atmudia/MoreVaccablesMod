using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.Player;

namespace MoreVaccablesMod.Patches;

[HarmonyPatch(typeof(AmmoSlotManager))]
public static class Patch_AmmoModel
{
    [HarmonyPatch(nameof(AmmoSlotManager.GetSlotMaxCount)), HarmonyPostfix]
    public static void GetSlotMaxCount(AmmoSlotManager __instance, int index, ref int __result)
    {
        // Get the ammo slot at this index
        var slot = __instance.Slots[index]; // or __instance.GetSlot(index) depending on the actual API
        if (slot == null) return;

        var id = slot.Id; // retrieve the IdentifiableType from the slot
        if (id == null) return;

        if (IsTarrEnabled.Value && id.ReferenceId.Equals("SlimeDefinition.Tarr"))
        {
            __result /= 2;
            return;
        }

        if (!LargoGroup.IsMember(id))
            return;

        __result /= 2;
    }
}
