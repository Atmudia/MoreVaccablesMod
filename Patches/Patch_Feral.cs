using System.Reflection;
using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.Player.PlayerItems;
using MelonLoader;
using UnityEngine;

namespace MoreVaccablesMod.Patches;

[HarmonyPatch(typeof(VacuumItem), nameof(VacuumItem.Expel), typeof(GameObject), typeof(bool), typeof(float), typeof(SlimeAppearance.AppearanceSaveSet))]
public class Patch_VacuumItem
{
    internal static MethodInfo IdentifiableTypeTryGetId;
    public static void Prefix(GameObject toExpel)
    {
        if (IdentifiableTypeTryGetId == null)
            return;

        IdentifiableType identifiableType = (IdentifiableType)IdentifiableTypeTryGetId.Invoke(null, [
            toExpel, null
        ]);
        // if (IdentifiableType.TryGetId(toExpel, out var type) && LargoGroup.IsMember(type))   //UNCOMMENT THIS LINE WHEN MELON GOT UPDATE
          
        if (identifiableType&& LargoGroup.IsMember(identifiableType))
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