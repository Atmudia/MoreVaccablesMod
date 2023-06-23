using HarmonyLib;

namespace MoreVaccablesMod.Patches;

[HarmonyPatch(typeof(AmmoModel), nameof(AmmoModel.GetSlotMaxCount))]
public static class AmmoModelGetSlotMaxCount
{
    public static void Postfix(AmmoModel __instance, IdentifiableType id, int index, ref int __result)
    {
        if (isTarrEnabled.Value && id.ReferenceId.Equals("SlimeDefinition.Tarr"))
        {
            __result /= 2;
            return;
        }

        if (!largoGroup.IsMember(id)) 
            return;
        __result /= 2;
    }
}