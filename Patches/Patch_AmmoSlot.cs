using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.Player;
using MelonLoader;

namespace MoreVaccablesMod.Patches;

[HarmonyPatch(typeof(AmmoSlot))]
public static class Patch_AmmoSlot
{
    private static readonly Dictionary<string, (int raw, int adjusted)> AmmoModels = new();
    
    [HarmonyPatch(nameof(AmmoSlot.Clear)), HarmonyPrefix]
    public static void Clear(AmmoSlot __instance)
    {
        var ammoSlotViewHolder = Patch_AmmoSlotViewHolder.ammoSlotViewHolder
            .FirstOrDefault(x => x._data.GetHashCode() == __instance.GetHashCode());
        if (!ammoSlotViewHolder) return;

        var ammoSlot = ammoSlotViewHolder.transform.Find("Ammo Slot").gameObject;
        ammoSlot.transform.Find("Icon").gameObject.SetActive(true);
        ammoSlot.transform.Find("FirstSlime").gameObject.SetActive(false);
        ammoSlot.transform.Find("SecondSlime").gameObject.SetActive(false);
    }

    [HarmonyPatch(nameof(AmmoSlot.MaxCount), MethodType.Getter), HarmonyPostfix]
    public static void MaxCount(AmmoSlot __instance, ref int __result)
    {
        if (!__instance.Id)
            return;
        __result = GetAmmoSlot(__instance.Id, __result);
    }
    
    public static int GetAmmoSlot(IdentifiableType ident, int defaultCount)
    {
        var idReferenceId = ident.ReferenceId;
        if (AmmoModels.TryGetValue(idReferenceId, out var value))
        {
            if (value.raw == defaultCount)
            {
                return value.adjusted;
            }
        }
        if (IsTarrEnabled.Value && idReferenceId.Equals("SlimeDefinition.Tarr"))
        {
            var adjusted = defaultCount / 2;
            AmmoModels[idReferenceId] = (raw: defaultCount, adjusted);
            return adjusted;
        }

        if (LargoGroup.IsMember(ident))
        {
            var adjusted = defaultCount / 2;
            AmmoModels[idReferenceId] = (raw: defaultCount, adjusted);
            return adjusted;

        }
        AmmoModels[idReferenceId] = (raw: defaultCount, adjusted: defaultCount);
        return defaultCount;
    }
}
