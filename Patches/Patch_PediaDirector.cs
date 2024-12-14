using HarmonyLib;

namespace MoreVaccablesMod.Patches;

[HarmonyPatch(typeof(PediaDirector), nameof(PediaDirector.Unlock), typeof(IdentifiableType))]
public static class PediaDirectorMaybeShowPopup
{
    public static bool Prefix(PediaDirector __instance, IdentifiableType identifiableType)
    {
        if (!identifiableType) return true;
        if (LargoGroup.IsMember(identifiableType))
        {
            __instance.Unlock(SRSingleton<SceneContext>.Instance.PlayerState.VacuumItem.LargoSlimePediaEntry);
            return false;
        }
        
        switch (identifiableType.ReferenceId)
        {
            case "SlimeDefinition.Lucky":
                __instance.Unlock(__instance.GetEntry(identifiableType));
                return false;
            case "SlimeDefinition.Tarr":
                __instance.Unlock(__instance.GetEntry(identifiableType));
                return false;
            case "SlimeDefinition.Gold":
                __instance.Unlock(__instance.GetEntry(identifiableType));
                return false;
            default:
                return true;
        }
    }
}