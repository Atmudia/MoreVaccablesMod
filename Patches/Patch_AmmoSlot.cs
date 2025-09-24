using System.Linq;
using HarmonyLib;
using MelonLoader;

namespace MoreVaccablesMod.Patches;

[HarmonyPatch(typeof(Il2CppMonomiPark.SlimeRancher.Player.AmmoSlot))]
public static class Patch_AmmoSlot
{
    [HarmonyPatch(nameof(Il2CppMonomiPark.SlimeRancher.Player.AmmoSlot.Clear)), HarmonyPrefix]
    public static void Clear(Il2CppMonomiPark.SlimeRancher.Player.AmmoSlot __instance)
    {
        var ammoSlotViewHolder = Patch_AmmoSlotViewHolder.ammoSlotViewHolder
            .FirstOrDefault(x => x._data.GetHashCode() == __instance.GetHashCode());
        if (!ammoSlotViewHolder) return;

        var ammoSlot = ammoSlotViewHolder.transform.Find("Ammo Slot").gameObject;
        ammoSlot.transform.Find("Icon").gameObject.SetActive(true);
        ammoSlot.transform.Find("FirstSlime").gameObject.SetActive(false);
        ammoSlot.transform.Find("SecondSlime").gameObject.SetActive(false);
    }
}
