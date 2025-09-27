using System.Collections.Generic;
using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.UI;
using MelonLoader;
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
        if (ammoSlotViewHolder.Find(x => !x))
            ammoSlotViewHolder.Clear();

        ammoSlotViewHolder.Add(__instance);

        var ammoSlot = __instance.transform.Find("Ammo Slot");
        var icon = ammoSlot.transform.Find("Icon");

        var slime1 = Object.Instantiate(icon, ammoSlot.transform).GetComponent<RectTransform>();
        slime1.name = "FirstSlime";
        slime1.GetComponent<Image>().enabled = true;
        slime1.sizeDelta /= 1.6f;
        slime1.anchoredPosition = new Vector2(9, 35);

        var slime2 = Object.Instantiate(icon, ammoSlot.transform).GetComponent<RectTransform>();
        slime2.name = "SecondSlime";
        slime2.GetComponent<Image>().enabled = true;
        slime2.sizeDelta /= 1.6f;
        slime2.anchoredPosition = new Vector2(-13.5f, -6.3f);

        slime1.gameObject.SetActive(false);
        slime2.gameObject.SetActive(false);
    }

    [HarmonyPatch(nameof(AmmoSlotViewHolder.UpdateAmmoDisplay)), HarmonyPrefix]
    public static void UpdateAmmoDisplay(AmmoSlotViewHolder __instance, Il2CppMonomiPark.SlimeRancher.Player.AmmoSlot data)
    {
        if (!LargoGroup.IsMember(data.Id) || data.Count == 0) return;

        var ammoSlotGO = __instance.transform.Find("Ammo Slot").gameObject;
        var firstSlime = ammoSlotGO.transform.Find("FirstSlime").gameObject;
        if (firstSlime.activeSelf) return;

        ammoSlotGO.transform.Find("Icon").gameObject.SetActive(false);
        var secondSlime = ammoSlotGO.transform.Find("SecondSlime").gameObject;

        var slimeDefinition = data.Id.Cast<SlimeDefinition>();
        var firstSlimeDef = slimeDefinition.BaseSlimes[0];
        var secondSlimeDef = slimeDefinition.BaseSlimes[1];

        if (firstSlimeDef) firstSlime.GetComponent<Image>().sprite = firstSlimeDef.icon;
        if (secondSlimeDef) secondSlime.GetComponent<Image>().sprite = secondSlimeDef.icon;

        firstSlime.SetActive(true);
        secondSlime.SetActive(true);
    }
}
