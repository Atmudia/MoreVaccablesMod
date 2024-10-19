using HarmonyLib;
using MelonLoader;

namespace MoreVaccablesMod.Patches;

[HarmonyPatch(typeof(SlimeAppearanceDirector))]
public static class Patch_SlimeAppearanceDirector
{
    [HarmonyPatch(nameof(GetSpecificSlimeAppearance)), HarmonyPrefix]
    public static bool GetSpecificSlimeAppearance(SlimeAppearanceDirector __instance, IdentifiableType slimeId, SlimeAppearance.AppearanceSaveSet saveSet, ref SlimeAppearance __result)
    {
        largoGroup ??= Get<IdentifiableTypeGroup>("LargoGroup");
        if (!largoGroup.IsMember(slimeId))
            return true;
        SetLargoIconAndPalette(slimeId.Cast<SlimeDefinition>());
        __result = __instance.GetChosenSlimeAppearance(slimeId);
        return false;
    }
}
