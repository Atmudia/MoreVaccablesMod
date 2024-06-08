using HarmonyLib;
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
        var identifiableTypeGroup = Get<IdentifiableTypeGroup>("VaccableNonLiquids");
        identifiableTypeGroup._memberGroups.Add(largoGroup);
        if (EntryPoint.isToysEnabled.Value)
            identifiableTypeGroup._memberGroups.Add(Get<IdentifiableTypeGroup>("ToyGroup"));
    }
}