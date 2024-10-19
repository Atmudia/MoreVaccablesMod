using HarmonyLib;
using MelonLoader;

namespace MoreVaccablesMod.Patches;

[HarmonyPatch(typeof(AmmoModel))]
public static class Patch_AmmoModel
{
    [HarmonyPatch(nameof(GetSlotMaxCount)), HarmonyPostfix]
    public static void GetSlotMaxCount(IdentifiableType id, ref int __result)
    {
        if (id.IsNull()) return;
        if (IsTarrEnabled.Value && id.ReferenceId.Equals("SlimeDefinition.Tarr"))
        {
            __result /= 2;
            return;
        }
        if (!LargoGroup.IsMember(id)) 
            return;
        __result /= 2;
    }
}