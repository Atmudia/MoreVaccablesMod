using HarmonyLib;

namespace MoreVaccablesMod.Patches;

[HarmonyPatch(typeof(SlimeAppearanceDirector))]
public static class Patch_SlimeAppearanceDirector
{
    [HarmonyPatch(nameof(SlimeAppearanceDirector.GetSpecificSlimeAppearance)), HarmonyPrefix]
    public static bool GetSpecificSlimeAppearance(SlimeAppearanceDirector __instance, IdentifiableType slimeId, SlimeAppearance.AppearanceSaveSet saveSet, ref SlimeAppearance __result)
    {
        LargoGroup ??= Get<IdentifiableTypeGroup>("LargoGroup");
        if (!LargoGroup.IsMember(slimeId))
            return true;
        SetLargoIconAndPalette(slimeId.Cast<SlimeDefinition>());
        __result = __instance.GetChosenSlimeAppearance(slimeId);
        return false;
    }
}
