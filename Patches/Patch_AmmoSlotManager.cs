using System.Linq;
using HarmonyLib;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using System;


namespace MoreVaccablesMod.Patches;

[HarmonyPatch]
public static class Patch_AmmoSlotManager
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Il2CppMonomiPark.SlimeRancher.Player.AmmoSlotManager), "MaybeAddToSlot")]
    public static void MaybeAddToSlot(Il2CppMonomiPark.SlimeRancher.Player.AmmoSlotManager __instance, IdentifiableType id, Identifiable identifiable, ref bool __result)
    {
        if (!__result || !LargoGroup.IsMember(id))
            return;

        var ammoSlotViewHolder = Patch_AmmoSlotViewHolder.ammoSlotViewHolder.FirstOrDefault(x => x._data.Id == id);
        if (ammoSlotViewHolder == null) return;

        var ammoSlot = ammoSlotViewHolder.transform.Find("Ammo Slot").gameObject;
        ammoSlot.transform.Find("Icon").gameObject.SetActive(false);

        var firstSlime = ammoSlot.transform.Find("FirstSlime").gameObject;
        var secondSlime = ammoSlot.transform.Find("SecondSlime").gameObject;

        var slimeDefinition = id.Cast<SlimeDefinition>();

        var firstSlimeDef = SRSingleton<SceneContext>.Instance.SlimeAppearanceDirector
            .GetChosenSlimeAppearance(slimeDefinition.BaseSlimes[0]);
        var secondSlimeDef = SRSingleton<SceneContext>.Instance.SlimeAppearanceDirector
            .GetChosenSlimeAppearance(slimeDefinition.BaseSlimes[1]);

        firstSlime.GetComponent<Image>().sprite = firstSlimeDef.Icon;
        secondSlime.GetComponent<Image>().sprite = secondSlimeDef.Icon;

        firstSlime.SetActive(true);
        secondSlime.SetActive(true);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Il2CppMonomiPark.SlimeRancher.Player.AmmoSlotManager), "Decrement", new Type[] { typeof(IdentifiableType), typeof(int) })]
    public static void DecrementSelectedAmmo(Il2CppMonomiPark.SlimeRancher.Player.AmmoSlotManager __instance, IdentifiableType id, int count = 1)
    {
        var ammoSlotViewHolder = Patch_AmmoSlotViewHolder.ammoSlotViewHolder
            .FirstOrDefault(x => x._data.Id == id);
        if (ammoSlotViewHolder == null) return;
        if (!LargoGroup.IsMember(ammoSlotViewHolder._data.Id)) return;

        var dataCount = ammoSlotViewHolder._data.Count - count;
        if (dataCount > 1) return;

        var ammoSlot = ammoSlotViewHolder.transform.Find("Ammo Slot").gameObject;
        ammoSlot.transform.Find("Icon").gameObject.SetActive(true);
        ammoSlot.transform.Find("FirstSlime").gameObject.SetActive(false);
        ammoSlot.transform.Find("SecondSlime").gameObject.SetActive(false);
    }
}
