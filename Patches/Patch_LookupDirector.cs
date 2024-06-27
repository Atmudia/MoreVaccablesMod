using System.Linq;
using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.Platform.AdditionalContent;
using Il2CppMonomiPark.SlimeRancher.Script.Util;
using UnityEngine;

namespace MoreVaccablesMod.Patches;

[HarmonyPatch(typeof(LookupDirector))]
public static class Patch_LookupDirector
{
    [HarmonyPatch(nameof(Awake)),HarmonyPrefix]
    public static void Awake(LookupDirector __instance)
    {
        nonSlimesGroup ??= Get<IdentifiableTypeGroup>("NonSlimesGroup");
        largoGroup ??= Get<IdentifiableTypeGroup>("LargoGroup");
        iconLargoPedia ??= Get<Sprite>("iconLargoPedia");
        nonSlimesGroup._memberGroups.Add(largoGroup);
        var nonLiquids = Get<IdentifiableTypeGroup>("VaccableNonLiquids");
        nonLiquids._memberGroups.Add(largoGroup);
        if (EntryPoint.isToysEnabled.Value)
            nonLiquids._memberGroups.Add(Get<IdentifiableTypeGroup>("ToyGroup"));
        var typeGroup = Get<IdentifiableTypeGroup>("VaccableBaseSlimeGroup");
           var identifiableTypeGroup = Get<IdentifiableTypeGroup>("VaccableBaseSlimeGroup");
        nonSlimesGroup._memberGroups.Add(identifiableTypeGroup);
        SlimeDefinition slimeGold = Get<SlimeDefinition>("Gold");
        slimeGold.prefab.GetComponent<Vacuumable>().size = VacuumableSize.NORMAL;
        foreach (var slimeAppearance in slimeGold.AppearancesDefault)
            slimeGold.SetPalette(slimeAppearance);
        if (slimeGold.prefab.TryGetComponentButWorking<GoldSlimeFlee>(out var goldSlimeFlee))
            Object.Destroy(goldSlimeFlee);
        identifiableTypeGroup._memberTypes.Add(slimeGold);
        nonSlimesGroup._memberTypes.Add(slimeGold);

        SlimeDefinition slimeLucky = Get<SlimeDefinition>("Lucky");
        foreach (var slimeAppearance in slimeLucky.AppearancesDefault)
            slimeLucky.SetPalette(slimeAppearance);
        if (slimeLucky.prefab.TryGetComponentButWorking<LuckySlimeFlee>(out var slimeLuckyFlee))
            Object.Destroy(slimeLuckyFlee);
        identifiableTypeGroup._memberTypes.Add(slimeLucky);
        nonSlimesGroup._memberTypes.Add(slimeLucky);
        if (isTarrEnabled.Value)
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
            identifiableTypeGroup._memberTypes.Add(slimeTarr); 
            nonSlimesGroup._memberTypes.Add(slimeTarr);
        }

        // nonSlimesGroup._runtimeObject = null;
      
        ColorUtility.TryParseHtmlString("#75d9ff", out var potColor);
        // var nonLiquids = Get<IdentifiableTypeGroup>("VaccableNonLiquids");
        var localizedString = LocalizationUtil.CreateByKey("Actor", "l.container_case");
        foreach (var identType in Resources.FindObjectsOfTypeAll<IdentifiableType>().Where(x => x.prefab != null))
        {
            if (identType.prefab.name.StartsWith("container"))
            {
                identType.prefab.GetComponent<Vacuumable>().size = VacuumableSize.NORMAL;
                identType.icon = iconContainer;
                identType.color = potColor;
                identType.localizedName = localizedString;
                nonLiquids._memberTypes.Add(identType);

            }
            
        }
        foreach (var identifiableType in new Il2CppSystem.Collections.Generic.List<IdentifiableType>(largoGroup.GetAllMembers()))
        {
            if (identifiableType.prefab != null)
                identifiableType.prefab.GetComponent<Vacuumable>().size = VacuumableSize.NORMAL;
            var type = identifiableType.TryCast<SlimeDefinition>();
            if (type == null)
                continue;
            if (type.AppearancesDefault == null)
            {
                LateActivation.Add(type);
                continue;
            }
            if (type.referenceId == null)
                continue;
            SetLargoIconAndPalette(type);
            // GameObject.CreatePrimitive(PrimitiveType.Sphere); 
            
        }

        if (isToysEnabled.Value)
        {
            var toyGroup = Get<IdentifiableTypeGroup>("ToyGroup");
            foreach (var identifiableTypePediaObject in Resources.FindObjectsOfTypeAll<AdditionalContentCatalog>().Where(x => x.Toys != null &&  x.Toys.Asset != null).SelectMany(x => x.Toys.Asset.Cast<IdentifiableTypePediaLinkMap>().IdentifiableTypePediaObjectMap.ToArray()))
            {
                toyGroup._memberTypes.Add(identifiableTypePediaObject.IdentifiableType);

            }
            toyGroup._runtimeObject = null;
            toyGroup.GetRuntimeObject();
            foreach (var toyId in toyGroup._memberTypes )
            {
                if (toyId.prefab != null)
                    toyId.prefab.GetComponent<Vacuumable>().size = VacuumableSize.NORMAL;
                if (toyId.icon != null)
                    SetPalette(toyId);
            }
        }

    }
}