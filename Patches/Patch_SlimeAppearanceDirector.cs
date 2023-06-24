using HarmonyLib;

namespace MoreVaccablesMod.Patches;

[HarmonyPatch(typeof(SlimeAppearanceDirector), nameof(SlimeAppearanceDirector.GetSpecificSlimeAppearance))]
public static class Patch_SlimeAppearanceDirector
{
    public static bool Prefix(SlimeAppearanceDirector __instance, IdentifiableType slimeId, SlimeAppearance.AppearanceSaveSet saveSet, ref SlimeAppearance __result)
    {
        largoGroup ??= Get<IdentifiableTypeGroup>("LargoGroup");
        if (!largoGroup.IsMember(slimeId))
            return true;
        SetLargoIconAndPalette(slimeId.Cast<SlimeDefinition>());
        __result = __instance.GetChosenSlimeAppearance(slimeId);
        return false;
    }
}