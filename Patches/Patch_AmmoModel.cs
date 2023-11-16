using HarmonyLib;
using MelonLoader;

namespace MoreVaccablesMod.Patches;

[HarmonyPatch(typeof(AmmoModel), nameof(AmmoModel.GetSlotMaxCount))]
public static class Patch_AmmoModel
{
    public static void Postfix(AmmoModel __instance, IdentifiableType id, int index, ref int __result)
    {
        
        if (id.IsNull()) return;
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