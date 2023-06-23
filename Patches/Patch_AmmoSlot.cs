using System.Linq;
using HarmonyLib;

namespace MoreVaccablesMod.Patches;

[HarmonyPatch(typeof(Ammo.Slot))]
public static class Patch_AmmoSlot
{
    [HarmonyPatch(nameof(Ammo.Slot.Clear))]
    public static void Clear(Ammo.Slot __instance)
    {
        var ammoSlotViewHolder = Patch_AmmoSlotViewHolder.ammoSlotViewHolder.FirstOrDefault(x =>x.data.GetHashCode() == __instance.GetHashCode());
        if (ammoSlotViewHolder == null) return;
        var ammoSlot = ammoSlotViewHolder.transform.Find("Ammo Slot").gameObject;
        ammoSlot.transform.Find("Icon").gameObject.SetActive(true);
        ammoSlot.transform.Find("FirstSlime").gameObject.SetActive(false);
        ammoSlot.transform.Find("SecondSlime").gameObject.SetActive(false);
    }
    [HarmonyPatch(nameof(Ammo.Slot.MaxCount), MethodType.Getter)]
    public static void MaxCount(Ammo.Slot __instance, ref int __result)
    {
        if (isTarrEnabled.Value && __instance.Id.ReferenceId.Equals("SlimeDefinition.Tarr"))
        {
            __result /= 2; 
            return;
        }
        if (!largoGroup.IsMember(__instance.Id)) 
            return;
        __result /= 2;
    }
}