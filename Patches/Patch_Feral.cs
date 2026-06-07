using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.Player;
using Il2CppMonomiPark.SlimeRancher.Player.PlayerItems;

namespace MoreVaccablesMod.Patches;

[HarmonyPatch(typeof(VacuumItem), nameof(VacuumItem.Expel), typeof(AmmoSlot.AmmoMetadata), typeof(bool), typeof(float), typeof(bool))]
public class Patch_VacuumItem
{
    public static void Postfix(AmmoSlot.AmmoMetadata metadata)
    {
        if (metadata.Id && LargoGroup.IsMember(metadata.Id))
            Patch_SlimeAppearanceApplicator.IsExpelled = true;
    }
}
[HarmonyPatch(typeof(SlimeFeral), nameof(SlimeFeral.Awake))]
public class Patch_SlimeFeral
{
    public static void Prefix(SlimeFeral __instance)
    {
        if (LargoGroup.IsMember(__instance.GetComponent<IdentifiableActor>().identType))
            __instance.gameObject.GetComponent<Vacuumable>().Size = VacuumableSize.LARGE;
    }
    public static void Postfix(SlimeFeral __instance)
    {
        if (LargoGroup.IsMember(__instance.GetComponent<IdentifiableActor>().identType))
            __instance.gameObject.GetComponent<Vacuumable>().Size = VacuumableSize.NORMAL;
    }
}