using System.Linq;
using HarmonyLib;
using UnityEngine.UI;
using Il2CppMonomiPark.SlimeRancher.Player;


namespace MoreVaccablesMod.Patches;

[HarmonyPatch]
public static class Patch_AmmoSlotManager
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Il2CppMonomiPark.SlimeRancher.Player.AmmoSlotManager), nameof(Il2CppMonomiPark.SlimeRancher.Player.AmmoSlotManager.MaybeAddToAnySlot))]
    public static void MaybeAddToAnySlot(ref bool __result, AmmoSlot.AmmoMetadata metadata)  => EditLargoSlot(__result, metadata);
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Il2CppMonomiPark.SlimeRancher.Player.AmmoSlotManager), nameof(Il2CppMonomiPark.SlimeRancher.Player.AmmoSlotManager.MaybeAddToSpecificSlot))]
    public static void MaybeAddToSpecificSlot(ref bool __result, AmmoSlot.AmmoMetadata metadata) => EditLargoSlot(__result, metadata);

    public static void EditLargoSlot(bool isAdded, AmmoSlot.AmmoMetadata metadata)
    {
        if (!isAdded || !LargoGroup.IsMember(metadata.Id))
            return;
        
        var ammoSlotViewHolder = Patch_AmmoSlotViewHolder.ammoSlotViewHolder.FirstOrDefault(x => x._data.Id == metadata.Id);
        if (ammoSlotViewHolder == null) return;
        
        var ammoSlot = ammoSlotViewHolder.transform.Find("Ammo Slot").gameObject;
        ammoSlot.transform.Find("Icon").gameObject.SetActive(false);
        
        var firstSlime = ammoSlot.transform.Find("FirstSlime").gameObject;
        var secondSlime = ammoSlot.transform.Find("SecondSlime").gameObject;
        
        var slimeDefinition = metadata.Id.Cast<SlimeDefinition>();
        
        var firstSlimeDef = SRSingleton<SceneContext>.Instance.SlimeAppearanceDirector
            .GetChosenSlimeAppearance(slimeDefinition.BaseSlimes[0]);
        var secondSlimeDef = SRSingleton<SceneContext>.Instance.SlimeAppearanceDirector
            .GetChosenSlimeAppearance(slimeDefinition.BaseSlimes[1]);
        
        firstSlime.GetComponent<Image>().sprite = firstSlimeDef.Icon;
        secondSlime.GetComponent<Image>().sprite = secondSlimeDef.Icon;
        
        firstSlime.SetActive(true);
        secondSlime.SetActive(true);
    }
    
    
}
