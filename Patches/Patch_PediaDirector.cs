using HarmonyLib;

namespace MoreVaccablesMod.Patches;

[HarmonyPatch(typeof(PediaDirector))]
public static class Patch_PediaDirector
{
    [HarmonyPatch(nameof(PediaDirector.Unlock), typeof(IdentifiableType)), HarmonyPrefix]
    public static bool Unlock(PediaDirector __instance, IdentifiableType identifiableType)
    {
        if (!identifiableType) return true;
        if (LargoGroup.IsMember(identifiableType))
        {
            __instance.Unlock(SRSingleton<SceneContext>.Instance.PlayerState.VacuumItem.LargoSlimePediaEntry);
            __instance.Unlock(SRSingleton<SceneContext>.Instance.PlayerState.VacuumItem.FeralSlimePediaEntry);
            return false;
        }
        
        switch (identifiableType.ReferenceId)
        {
            case "SlimeDefinition.Lucky":
            case "SlimeDefinition.Tarr":
            case "SlimeDefinition.Gold":
            case "SlimeDefinition.Shadow":
                __instance.Unlock(__instance.GetEntry(identifiableType));
                return false;
            default:
                return true;
        }
    }
}