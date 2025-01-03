using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.Persist;
using Il2CppMonomiPark.SlimeRancher.UI;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace MoreVaccablesMod.Patches;

[HarmonyPatch(typeof(StorageSlotUI))]
public static class Patch_StorageSlotUI
{
    [HarmonyPatch(nameof(SetImageSprite)), HarmonyPrefix]
    public static void SetImageSprite(StorageSlotUI __instance, Image image, Sprite sprite)
    {
        var identifiableType = __instance.GetCurrentId();
        if (!LargoGroup.IsMember(identifiableType)) 
            return;
        var ammoSlot = __instance.bar.transform.gameObject;
        var icon = ammoSlot.transform.Find("Icon").gameObject;
        var firstSlime = ammoSlot.transform.Find("FirstSlime");
        var secondSlime = ammoSlot.transform.Find("SecondSlime");
        if (!firstSlime)
        {
            var slime1 = Object.Instantiate(icon, ammoSlot.transform).GetComponent<RectTransform>();
            slime1.GetComponent<Image>().enabled = true;
            slime1.name = "FirstSlime";
            slime1.sizeDelta /= 1.6f;
            slime1.anchoredPosition = new Vector2(9, 35);
            firstSlime = slime1;
        }
        if (!secondSlime)
        {
            var slime1 = Object.Instantiate(icon, ammoSlot.transform).GetComponent<RectTransform>();
            slime1.GetComponent<Image>().enabled = true;
            slime1.name = "SecondSlime";
            slime1.sizeDelta /= 1.6f;
            slime1.anchoredPosition = new Vector2(-13.5f, -6.3f);
            secondSlime = slime1;
        }
        var slimeDefinition = identifiableType.Cast<SlimeDefinition>();
        var firstSlimeDefinition = slimeDefinition.BaseSlimes[0];
        var secondSlimeDefinition = slimeDefinition.BaseSlimes[1];
        if (firstSlimeDefinition) firstSlime.GetComponent<Image>().sprite = firstSlimeDefinition.icon;
        if (secondSlimeDefinition) secondSlime.GetComponent<Image>().sprite = secondSlimeDefinition.icon;
        firstSlime.gameObject.SetActive(true);
        secondSlime.gameObject.SetActive(true);
        icon.gameObject.SetActive(false);
    }

    [HarmonyPatch(nameof(Clear)), HarmonyPrefix]
    public static void Clear(StorageSlotUI __instance)
    {
        var ammoSlot = __instance.bar.transform.gameObject;
        var icon = ammoSlot.transform.Find("Icon").gameObject;
        var firstSlime = ammoSlot.transform.Find("FirstSlime");
        var secondSlime = ammoSlot.transform.Find("SecondSlime");
        if (!firstSlime && !secondSlime)
        {
            return;
        }
        firstSlime.gameObject.SetActive(false);
        secondSlime.gameObject.SetActive(false);
        icon.gameObject.SetActive(true);

        
    }
}