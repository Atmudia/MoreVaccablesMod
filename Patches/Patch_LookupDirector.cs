using System.Linq;
using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.Platform.AdditionalContent;
using Il2CppMonomiPark.SlimeRancher.Script.Util;
using Il2CppMonomiPark.SlimeRancher.Slime.Shadow;
using UnityEngine;

namespace MoreVaccablesMod.Patches;

[HarmonyPatch(typeof(LookupDirector))]
public static class Patch_LookupDirector
{
    [HarmonyPatch(nameof(Awake)),HarmonyPostfix]
    public static void Awake(LookupDirector __instance)
    {
        VaccableBaseSlimeGroup ??= Get<IdentifiableTypeGroup>("VaccableBaseSlimeGroup");
        NonSlimesGroup ??= Get<IdentifiableTypeGroup>("NonSlimesGroup");
        
        LargoGroup ??= Get<IdentifiableTypeGroup>("LargoGroup");
        IconLargoPedia ??= Get<Sprite>("iconLargoPedia");
        
        foreach (var identifiableType in new Il2CppSystem.Collections.Generic.List<IdentifiableType>(LargoGroup.GetAllMembers()))
        {
            if (identifiableType.prefab)
                identifiableType.prefab.GetComponent<Vacuumable>().Size = VacuumableSize.NORMAL;
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
        
        VaccableBaseSlimeGroup._memberGroups.Add(LargoGroup);
        
        SlimeDefinition slimeGold = Get<SlimeDefinition>("Gold");
        slimeGold.prefab.GetComponent<Vacuumable>().Size = VacuumableSize.NORMAL;
        foreach (var slimeAppearance in slimeGold.AppearancesDefault)
            slimeGold.SetPalette(slimeAppearance);

        VaccableBaseSlimeGroup._memberTypes.Add(slimeGold);
        
        SlimeDefinition slimeLucky = Get<SlimeDefinition>("Lucky");
        foreach (var slimeAppearance in slimeLucky.AppearancesDefault)
        {
            ColorUtility.TryParseHtmlString("#b0a5a2", out var luckyColor);
            var colorPalette = slimeAppearance.ColorPalette;
            colorPalette.Ammo = luckyColor;
            slimeLucky.color = luckyColor;
            slimeAppearance._colorPalette = colorPalette;
        }

        VaccableBaseSlimeGroup._memberTypes.Add(slimeLucky);

        SlimeDefinition slimeShadow = Get<SlimeDefinition>("Shadow");
        foreach (var slimeAppearance in slimeLucky.AppearancesDefault)
        {
            ColorUtility.TryParseHtmlString("#15314d", out var shadowColor);
            var colorPalette = slimeAppearance.ColorPalette;
            colorPalette.Ammo = shadowColor;
            slimeShadow.color = shadowColor;
            slimeAppearance._colorPalette = colorPalette;
        }
        VaccableBaseSlimeGroup._memberTypes.Add(slimeShadow);
        if (IsTarrEnabled.Value)
        {
            SlimeDefinition slimeTarr = Get<SlimeDefinition>("Tarr");
            slimeTarr.prefab.GetComponent<Vacuumable>().Size = VacuumableSize.NORMAL;
            foreach (var slimeAppearance in slimeTarr.AppearancesDefault)
            {
                if (slimeAppearance.SaveSet == SlimeAppearance.AppearanceSaveSet.CLASSIC)
                {
                    slimeAppearance._icon = Get<Sprite>("iconSlimeTarr");
                }
                slimeTarr.SetPalette(slimeAppearance);
            }
            slimeTarr.icon = Get<Sprite>("iconSlimeTarr");
            VaccableBaseSlimeGroup._memberTypes.Add(slimeTarr);
        }
        ColorUtility.TryParseHtmlString("#75d9ff", out var potColor);
        var localizedString = LocalizationUtil.CreateByKey("Actor", "l.pot");
        foreach (var identType in Resources.FindObjectsOfTypeAll<IdentifiableType>().Where(x => x.prefab && x.prefab.name.StartsWith("container")))
        {
            identType.prefab.GetComponent<Vacuumable>().Size = VacuumableSize.NORMAL;
            identType.icon = IconContainer;
            identType.color = potColor;
            identType.localizedName = localizedString;
            VaccableBaseSlimeGroup._memberTypes.Add(identType);
            
        }

        if (IsSlimeFleeingEnabled.Value)
        {
            if (slimeGold.prefab.TryGetComponentButWorking(out GoldSlimeFlee goldSlimeFlee))
                Object.Destroy(goldSlimeFlee);
            if (slimeLucky.prefab.TryGetComponentButWorking(out LuckySlimeFlee slimeLuckyFlee))
                Object.Destroy(slimeLuckyFlee);
            if (slimeShadow.prefab.TryGetComponentButWorking(out ShadowSlimeScatter slimeScatter))
                Object.Destroy(slimeScatter);
            
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
                    toyId.prefab.GetComponent<Vacuumable>().Size = VacuumableSize.NORMAL;
                if (toyId.icon)
                    SetPalette(toyId);
            }
            VaccableBaseSlimeGroup._memberGroups.Add(toyGroup);
        }
        NonSlimesGroup._memberGroups.Add(VaccableBaseSlimeGroup);
        __instance._identifiableTypeGroupMap[NonSlimesGroup]._memberGroups = NonSlimesGroup._memberGroups;
        __instance._identifiableTypeGroupMap[NonSlimesGroup]._memberTypes = NonSlimesGroup._memberTypes;
        __instance._identifiableTypeGroupMap[VaccableBaseSlimeGroup]._memberTypes = VaccableBaseSlimeGroup._memberTypes;
        __instance._identifiableTypeGroupMap[VaccableBaseSlimeGroup]._memberGroups = VaccableBaseSlimeGroup._memberGroups;
    }
    
    
}