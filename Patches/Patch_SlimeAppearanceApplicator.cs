using HarmonyLib;

namespace MoreVaccablesMod.Patches;

[HarmonyPatch(typeof(SlimeAppearanceApplicator))]
public class Patch_SlimeAppearanceApplicator
{
    internal static bool IsExpelled;
    [HarmonyPatch(nameof(SlimeAppearanceApplicator.SetExpression)), HarmonyPrefix]
    public static void SetExpression(SlimeAppearanceApplicator __instance)
    {
        if (!__instance.Appearance && IsExpelled)
        {
            IsExpelled = false;
            __instance.Awake();
        }
    }
}