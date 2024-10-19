using HarmonyLib;

namespace MoreVaccablesMod.Patches;

[HarmonyPatch(typeof(SlimeAppearanceApplicator))]
public class Patch_SlimeAppearanceApplicator
{
    public static bool IsExpeled;
    [HarmonyPatch(nameof(SetExpression)), HarmonyPrefix]
    public static void SetExpression(SlimeAppearanceApplicator __instance)
    {
        if (!__instance.Appearance && IsExpeled)
        {
            IsExpeled = false;
            __instance.Awake();
        }
    }
}