using System.Collections.Generic;
using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MoreVaccablesMod.Patches;

[HarmonyPatch(typeof(AmmoSlotViewHolder))]
public static class Patch_AmmoSlotViewHolder
{
    public static List<AmmoSlotViewHolder> ammoSlotViewHolder = new List<AmmoSlotViewHolder>();
    
    [HarmonyPatch(nameof(AmmoSlotViewHolder.Awake)), HarmonyPrefix]
    public static void Awake(AmmoSlotViewHolder __instance)
    {
        if (ammoSlotViewHolder.Find(x => x == null))
            ammoSlotViewHolder.Clear();
        ammoSlotViewHolder.Add(__instance);
        var ammoSlot = __instance.transform.Find("Ammo Slot");
        var icon = ammoSlot.transform.Find("Icon");

        var slime1 = Object.Instantiate(icon, ammoSlot.transform).GetComponent<RectTransform>();
        slime1.name = "FirstSlime";
        slime1.sizeDelta /= 1.6f;
        slime1.anchoredPosition = new Vector2(9, 35);
        var slime2 = Object.Instantiate(icon, ammoSlot.transform).GetComponent<RectTransform>();
        slime2.name = "SecondSlime";
        slime2.sizeDelta /= 1.6f;
        slime2.anchoredPosition = new Vector2(-9, -9);
        slime1.gameObject.SetActive(false);
        slime2.gameObject.SetActive(false); 
    }
    [HarmonyPatch(nameof(AmmoSlotViewHolder.UpdateAmmoDisplay)), HarmonyPrefix]
    public static void UpdateAmmoDisplay(AmmoSlotViewHolder __instance, Ammo.Slot data)
    {
        if (!largoGroup.IsMember(data.id)) return;
        if (data.count == 0) return;
        var ammoSlot = __instance.transform.Find("Ammo Slot").gameObject;
        var firstSlime = ammoSlot.transform.Find("FirstSlime").gameObject;
        if (firstSlime.activeSelf) return;
        ammoSlot.transform.Find("Icon").gameObject.SetActive(false);
        var secondSlime = ammoSlot.transform.Find("SecondSlime").gameObject;
        var slimeDefinition = data.id.Cast<SlimeDefinition>();
        var firstSlimeDefinition = slimeDefinition.BaseSlimes[0];
        var secondSlimeDefinition = slimeDefinition.BaseSlimes[1];
        firstSlime.GetComponent<Image>().sprite = firstSlimeDefinition.icon;
        secondSlime.GetComponent<Image>().sprite = secondSlimeDefinition.icon;
        firstSlime.gameObject.SetActive(true);
        secondSlime.gameObject.SetActive(true);
    }
}