using System.Linq;
using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.Platform.AdditionalContent;
using Il2CppMonomiPark.SlimeRancher.Script.Util;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using UnityEngine;

namespace MoreVaccablesMod.Patches;

[HarmonyPatch(typeof(LookupDirector))]
public static class Patch_LookupDirector
{
    [HarmonyPatch(nameof(Awake)),HarmonyPostfix]
    public static void Awake(LookupDirector __instance)
    {

        NonSlimesGroup ??= Get<IdentifiableTypeGroup>("VaccableBaseSlimeGroup");
        LargoGroup ??= Get<IdentifiableTypeGroup>("LargoGroup");
        IconLargoPedia ??= Get<Sprite>("iconLargoPedia");
        
        foreach (var identifiableType in new Il2CppSystem.Collections.Generic.List<IdentifiableType>(LargoGroup.GetAllMembers()))
        {
            if (identifiableType.prefab)
                identifiableType.prefab.GetComponent<Vacuumable>().size = VacuumableSize.NORMAL;
            var type = identifiableType.TryCast<SlimeDefinition>();
            if (!type)
                continue;
            if (type.AppearancesDefault == null)
            {
                LateActivation.Add(type);
                continue;
            }
            if (type.referenceId == null)
                continue;
            SetLargoIconAndPalette(type);


        }
        
        NonSlimesGroup._memberGroups.Add(LargoGroup);
        
        SlimeDefinition slimeGold = Get<SlimeDefinition>("Gold");
        slimeGold.prefab.GetComponent<Vacuumable>().size = VacuumableSize.NORMAL;
        foreach (var slimeAppearance in slimeGold.AppearancesDefault)
            slimeGold.SetPalette(slimeAppearance);
        if (slimeGold.prefab.TryGetComponentButWorking<GoldSlimeFlee>(out var goldSlimeFlee))
            Object.Destroy(goldSlimeFlee);
        NonSlimesGroup._memberTypes.Add(slimeGold);
        
        SlimeDefinition slimeLucky = Get<SlimeDefinition>("Lucky");
        foreach (var slimeAppearance in slimeLucky.AppearancesDefault)
        {
            ColorUtility.TryParseHtmlString("#b0a5a2", out var luckyColor);
            var colorPalette = slimeAppearance.ColorPalette;
            colorPalette.Ammo = luckyColor;
            slimeLucky.color = luckyColor;
            slimeAppearance._colorPalette = colorPalette;
        }
        if (slimeLucky.prefab.TryGetComponentButWorking<LuckySlimeFlee>(out var slimeLuckyFlee))
            Object.Destroy(slimeLuckyFlee);
        NonSlimesGroup._memberTypes.Add(slimeLucky);
        if (IsTarrEnabled.Value)
        {
            SlimeDefinition slimeTarr = Get<SlimeDefinition>("Tarr");
            slimeTarr.prefab.GetComponent<Vacuumable>().size = VacuumableSize.NORMAL;
            foreach (var slimeAppearance in slimeTarr.AppearancesDefault)
            {
                if (slimeAppearance.SaveSet == SlimeAppearance.AppearanceSaveSet.CLASSIC)
                {
                    slimeAppearance._icon = Get<Sprite>("iconSlimeTarr");
                }
                slimeTarr.SetPalette(slimeAppearance);
            }
            slimeTarr.icon = Get<Sprite>("iconSlimeTarr");
            NonSlimesGroup._memberTypes.Add(slimeTarr);
        }
        ColorUtility.TryParseHtmlString("#75d9ff", out var potColor);
        var localizedString = LocalizationUtil.CreateByKey("Actor", "l.pot");
        foreach (var identType in Resources.FindObjectsOfTypeAll<IdentifiableType>().Where(x => x.prefab))
        {
            if (identType.prefab.name.StartsWith("container"))
            {
                identType.prefab.GetComponent<Vacuumable>().size = VacuumableSize.NORMAL;
                identType.icon = IconContainer;
                identType.color = potColor;
                identType.localizedName = localizedString;
                NonSlimesGroup._memberTypes.Add(identType);
        
            }
            
        }
        if (IsToysEnabled.Value)
        {
            var toyGroup = Get<IdentifiableTypeGroup>("ToyGroup");
            foreach (var identifiableTypePediaObject in Resources.FindObjectsOfTypeAll<AdditionalContentCatalog>().Where(x => x.Toys != null &&  x.Toys.Asset != null).SelectMany(x => x.Toys.Asset.Cast<IdentifiableTypePediaLinkMap>().IdentifiableTypePediaObjectMap.ToArray()))
            {
                toyGroup._memberTypes.Add(identifiableTypePediaObject.IdentifiableType);
                __instance._identifiableTypeGroupMap[toyGroup]._memberTypes.Add(identifiableTypePediaObject.IdentifiableType);
            
            }
            foreach (var toyId in toyGroup._memberTypes )
            {
                if (toyId.prefab)
                    toyId.prefab.GetComponent<Vacuumable>().size = VacuumableSize.NORMAL;
                if (toyId.icon)
                    SetPalette(toyId);
            }
            NonSlimesGroup._memberGroups.Add(toyGroup);
        }
        NonSlimesGroup._memberGroups.Add(LargoGroup);
        
        __instance._identifiableTypeGroupMap[NonSlimesGroup]._memberTypes = NonSlimesGroup._memberTypes;
        __instance._identifiableTypeGroupMap[NonSlimesGroup]._memberGroups = NonSlimesGroup._memberGroups;
    }
    
    
}