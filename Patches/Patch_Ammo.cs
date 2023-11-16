using System.Linq;
using HarmonyLib;
using MelonLoader;
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
        MelonLogger.Msg("test");
        var ammoSlotViewHolder = Patch_AmmoSlotViewHolder.ammoSlotViewHolder.Find(x => x.data.Id == id);
        var ammoSlot = ammoSlotViewHolder.transform.Find("Ammo Slot").gameObject;
        ammoSlot.transform.Find("Icon").gameObject.SetActive(false);
        var firstSlime = ammoSlot.transform.Find("FirstSlime").gameObject;
        var secondSlime = ammoSlot.transform.Find("SecondSlime").gameObject;
        var slimeDefinition = id.Cast<SlimeDefinition>();
        
        var firstSlimeDefinition = SRSingleton<SceneContext>.Instance.SlimeAppearanceDirector.GetChosenSlimeAppearance(slimeDefinition.BaseSlimes[0]);
        var secondSlimeDefinition = SRSingleton<SceneContext>.Instance.SlimeAppearanceDirector.GetChosenSlimeAppearance(slimeDefinition.BaseSlimes[1]);
        firstSlime.GetComponent<Image>().sprite = firstSlimeDefinition.Icon;
        secondSlime.GetComponent<Image>().sprite = secondSlimeDefinition.Icon;
        firstSlime.gameObject.SetActive(true);
        secondSlime.gameObject.SetActive(true);
    }
    [HarmonyPatch(nameof(Ammo.DecrementSelectedAmmo)), HarmonyPrefix]
    public static void DecrementSelectedAmmo(Ammo __instance, int amount)
    {
        var ammoSlotViewHolder = Patch_AmmoSlotViewHolder.ammoSlotViewHolder.FirstOrDefault(x =>x.data.Index == __instance.SelectedAmmoIndex);
        if (ammoSlotViewHolder == null) return;
        if (!largoGroup.IsMember(ammoSlotViewHolder.data.Id))
            return;
        var dataCount = ammoSlotViewHolder.data.Count - amount;
        if (dataCount > 1)
            return;
        var ammoSlot = ammoSlotViewHolder.transform.Find("Ammo Slot").gameObject;
        ammoSlot.transform.Find("Icon").gameObject.SetActive(true);
        ammoSlot.transform.Find("FirstSlime").gameObject.SetActive(false);
        ammoSlot.transform.Find("SecondSlime").gameObject.SetActive(false);
        MelonLogger.Msg($"test {nameof(DecrementSelectedAmmo)}");
    }
    
}