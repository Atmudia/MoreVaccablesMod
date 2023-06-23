using System.Linq;
using HarmonyLib;
using UnityEngine.UI;

namespace MoreVaccablesMod.Patches;

[HarmonyPatch(typeof(Ammo))]
public static class Patch_Ammo
{
    [HarmonyPatch(nameof(Ammo.MaybeAddToSlot)), HarmonyPrefix]
    public static void MaybeAddToSlot(Ammo __instance, IdentifiableType id, Identifiable identifiable, ref bool __result)
    {
        if (!__result || !largoGroup.IsMember(id)) 
                return;
        var ammoSlotViewHolder = Patch_AmmoSlotViewHolder.ammoSlotViewHolder.Find(x => x.data.id == id);
        var ammoSlot = ammoSlotViewHolder.transform.Find("Ammo Slot").gameObject;
        ammoSlot.transform.Find("Icon").gameObject.SetActive(false);
        var firstSlime = ammoSlot.transform.Find("FirstSlime").gameObject;
        var secondSlime = ammoSlot.transform.Find("SecondSlime").gameObject;
        var slimeDefinition = id.Cast<SlimeDefinition>();
        var firstSlimeDefinition = slimeDefinition.BaseSlimes[0];
        var secondSlimeDefinition = slimeDefinition.BaseSlimes[1];
        firstSlime.GetComponent<Image>().sprite = firstSlimeDefinition.icon;
        secondSlime.GetComponent<Image>().sprite = secondSlimeDefinition.icon;
        firstSlime.gameObject.SetActive(true);
        secondSlime.gameObject.SetActive(true);
    }
    [HarmonyPatch(nameof(Ammo.DecrementSelectedAmmo)), HarmonyPrefix]
    public static void DecrementSelectedAmmo(Ammo __instance, int amount)
    {
        var ammoSlotViewHolder = Patch_AmmoSlotViewHolder.ammoSlotViewHolder.FirstOrDefault(x =>x.data.index == __instance.selectedAmmoIdx);
        if (ammoSlotViewHolder == null) return;
        if (!largoGroup.IsMember(ammoSlotViewHolder.data.id))
            return;
        var dataCount = ammoSlotViewHolder.data.count - amount;
        if (dataCount > 1)
            return;
        var ammoSlot = ammoSlotViewHolder.transform.Find("Ammo Slot").gameObject;
        ammoSlot.transform.Find("Icon").gameObject.SetActive(true);
        ammoSlot.transform.Find("FirstSlime").gameObject.SetActive(false);
        ammoSlot.transform.Find("SecondSlime").gameObject.SetActive(false);
    }
    
}