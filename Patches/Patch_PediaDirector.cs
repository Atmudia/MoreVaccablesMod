using HarmonyLib;

namespace MoreVaccablesMod.Patches;

[HarmonyPatch(typeof(PediaDirector), nameof(PediaDirector.MaybeShowPopup), typeof(IdentifiableType))]
public static class PediaDirectorMaybeShowPopup
{
    public static bool Prefix(PediaDirector __instance, IdentifiableType identId)
    {
        if (identId.IsNull()) return true;
        if (largoGroup.IsMember(identId))
        {
            __instance.MaybeShowPopup(SRSingleton<SceneContext>.Instance.PlayerState.Vacuum.largoSlimePediaEntry);
            return false;
        }

        switch (identId.ReferenceId)
        {
            case "SlimeDefinition.Lucky":
                __instance.MaybeShowPopup(__instance.GetPediaId(identId));
                return false;
            case "SlimeDefinition.Tarr":
                __instance.MaybeShowPopup(__instance.GetPediaId(identId));
                return false;
            case "SlimeDefinition.Gold":
                __instance.MaybeShowPopup(__instance.GetPediaId(identId));
                return false;
            default:
                return true;
        }
    }
}