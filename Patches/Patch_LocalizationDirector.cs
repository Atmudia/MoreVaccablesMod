using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.UI.Localization;
using MelonLoader;
using UnityEngine;
using System.Collections;

namespace MoreVaccablesMod.Patches;

[HarmonyPatch(typeof(LocalizationDirector), nameof(LocalizationDirector.LoadTables))]
public static class LocalizationDirectorLoadTable
{
    public static void Postfix(LocalizationDirector __instance)
    {
        MelonCoroutines.Start(LoadTable(__instance));
    }

    public static IEnumerator LoadTable(LocalizationDirector localizationDirector)
    {
        yield return new WaitForSeconds(0.1f);
        localizationDirector.Tables["Actor"].AddEntry("l.container_case", "Case");
    }
}