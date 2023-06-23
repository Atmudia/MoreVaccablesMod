using HarmonyLib;

namespace MoreVaccablesMod.Patches;

[HarmonyPatch(typeof(AmmoModel), nameof(AmmoModel.GetSlotMaxCount))]
public static class AmmoModelGetSlotMaxCount
{
    public static void Postfix(AmmoModel __instance, IdentifiableType id, int index, ref int __result)
    {
        if (!largoGroup.IsMember(id) || id.ReferenceId.Equals("SlimeDefinition.Tarr")) 
            return;
        __result /= 2;
    }
}