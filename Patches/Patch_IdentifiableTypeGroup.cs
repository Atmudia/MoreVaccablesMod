using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.Player.PlayerItems;
using MelonLoader;

namespace MoreVaccablesMod.Patches;

// [HarmonyPatch(typeof(IdentifiableTypeGroup))]
// public class Patch_IdentifiableTypeGroup
// {
//     [HarmonyPatch(nameof(IsMember)),HarmonyPrefix]
//
//     public static void IsMember(IdentifiableTypeGroup __instance, IdentifiableType identType, ref bool __result)
//     {
//        __result = true;
//     }
//     [HarmonyPatch(nameof(IdentifiableTypeGroup.IsBaseMember)),HarmonyPrefix]
//
//     public static void IsBaseMember(IdentifiableTypeGroup __instance, IdentifiableType identType, ref bool __result)
//     {
//         __result = true;
//     }
// }
