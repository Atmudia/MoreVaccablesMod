using System.Linq;
using HarmonyLib;
using MelonLoader;

namespace MoreVaccablesMod.Patches;

[HarmonyPatch(typeof(Ammo.Slot))]
public static class Patch_AmmoSlot
{
    [HarmonyPatch(nameof(Ammo.Slot.Clear)), HarmonyPrefix]
    public static void Clear(Ammo.Slot __instance)
    {
        var ammoSlotViewHolder = Patch_AmmoSlotViewHolder.ammoSlotViewHolder.FirstOrDefault(x =>x.data.GetHashCode() == __instance.GetHashCode());
        if (!ammoSlotViewHolder) return;
        var ammoSlot = ammoSlotViewHolder.transform.Find("Ammo Slot").gameObject;
        ammoSlot.transform.Find("Icon").gameObject.SetActive(true);
        ammoSlot.transform.Find("FirstSlime").gameObject.SetActive(false);
        ammoSlot.transform.Find("SecondSlime").gameObject.SetActive(false);
    }
}