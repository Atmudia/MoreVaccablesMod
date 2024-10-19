using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.Player.PlayerItems;
using MelonLoader;
using UnityEngine;

namespace MoreVaccablesMod.Patches;

[HarmonyPatch(typeof(VacuumItem), nameof(VacuumItem.Expel), typeof(GameObject), typeof(bool), typeof(float), typeof(SlimeAppearance.AppearanceSaveSet))]
public class Patch_VacuumItem
{
    public static void Prefix(GameObject toExpel)
    {
        if (IdentifiableType.TryGetId(toExpel, out var type) && LargoGroup.IsMember(type))
            Patch_SlimeAppearanceApplicator.IsExpeled = true;
    }
}
[HarmonyPatch(typeof(SlimeFeral), nameof(SlimeFeral.Awake))]
public class Patch_SlimeFeral
{
    public static void Prefix(SlimeFeral __instance)
    {

        if (LargoGroup.IsMember(__instance.GetComponent<IdentifiableActor>().identType))
        {
            __instance.gameObject.GetComponent<Vacuumable>().size = VacuumableSize.LARGE;
        }
    }
    public static void Postfix(SlimeFeral __instance)
    {

        if (LargoGroup.IsMember(__instance.GetComponent<IdentifiableActor>().identType))
        {
            __instance.gameObject.GetComponent<Vacuumable>().size = VacuumableSize.NORMAL;
        }
    }
}