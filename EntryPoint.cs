global using Il2Cpp;
global using static MoreVaccablesMod.EntryPoint;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppMonomiPark.SlimeRancher.Script.Util;
using Il2CppMonomiPark.SlimeRancher.UI.Localization;
using MelonLoader;
using MoreVaccablesMod;
using UnityEngine;
using Object = UnityEngine.Object;
[assembly: MelonInfo(typeof(EntryPoint), "MoreVaccablesMod", "1.0.6", "KomiksPL", "https://www.nexusmods.com/slimerancher2/mods/42")]
namespace MoreVaccablesMod;

public class EntryPoint : MelonMod
{
    internal static IdentifiableTypeGroup largoGroup;
    internal static IdentifiableTypeGroup nonSlimesGroup;
    private static Sprite iconLargoPedia;
    private static Sprite iconContainer;
    private MelonPreferences_Category MoreVaccablesMod;
    internal static MelonPreferences_Entry<bool> isTarrEnabled;

    public static List<SlimeDefinition> LateActivation = new List<SlimeDefinition>();

    public override void OnInitializeMelon()
    {
        MoreVaccablesMod = MelonPreferences.CreateCategory(nameof(MoreVaccablesMod));
        isTarrEnabled = MoreVaccablesMod.CreateEntry<bool>("isTarrEnabled", true, "Is Tarr Enabled", "Should More Vaccable can vac Tarr Slime?");
    }
    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        if (sceneName.Equals("zoneCore"))
        {
            foreach (var identifiableType in LateActivation)
            {
                if (identifiableType.AppearancesDefault == null)
                {
                    MelonLogger.Msg($"Can't add modded largo to be vaccable: {identifiableType.ReferenceId}");
                    continue;
                }
                if (identifiableType.prefab != null)
                    identifiableType.prefab.GetComponent<Vacuumable>().size = Vacuumable.Size.NORMAL;
                SetLargoIconAndPalette(identifiableType);
            }
        }
        
        if (!sceneName.Equals("GameCore")) return;
        nonSlimesGroup ??= Get<IdentifiableTypeGroup>("NonSlimesGroup");
        largoGroup ??= Get<IdentifiableTypeGroup>("LargoGroup");
        iconLargoPedia ??= Get<Sprite>("iconLargoPedia");
        iconContainer ??= ConvertSprite(LoadImage("MoreVaccablesMod.iconContainer.png"));
        var identifiableTypeGroup = Get<IdentifiableTypeGroup>("VaccableBaseSlimeGroup");
        nonSlimesGroup.memberGroups.Add(identifiableTypeGroup);
        SlimeDefinition slimeGold = Get<SlimeDefinition>("Gold");
        slimeGold.prefab.GetComponent<Vacuumable>().size = Vacuumable.Size.NORMAL;
        foreach (var slimeAppearance in slimeGold.AppearancesDefault)
            slimeGold.SetPalette(slimeAppearance);
        if (slimeGold.prefab.TryGetComponentButBetter<GoldSlimeFlee>(out var goldSlimeFlee))
            Object.Destroy(goldSlimeFlee);
        identifiableTypeGroup.memberTypes.Add(slimeGold);
        nonSlimesGroup.memberTypes.Add(slimeGold);

        SlimeDefinition slimeLucky = Get<SlimeDefinition>("Lucky");
        foreach (var slimeAppearance in slimeLucky.AppearancesDefault)
            slimeLucky.SetPalette(slimeAppearance);
        if (slimeLucky.prefab.TryGetComponentButBetter<LuckySlimeFlee>(out var slimeLuckyFlee))
            Object.Destroy(slimeLuckyFlee);
        identifiableTypeGroup.memberTypes.Add(slimeLucky);
        nonSlimesGroup.memberTypes.Add(slimeLucky);
        if (isTarrEnabled.Value)
        {
            SlimeDefinition slimeTarr = Get<SlimeDefinition>("Tarr");
            slimeTarr.prefab.GetComponent<Vacuumable>().size = Vacuumable.Size.NORMAL;
            foreach (var slimeAppearance in slimeTarr.AppearancesDefault)
            {
                if (slimeAppearance.SaveSet == SlimeAppearance.AppearanceSaveSet.CLASSIC)
                {
                    slimeAppearance._icon = Get<Sprite>("iconSlimeTarr");
                }
                slimeTarr.SetPalette(slimeAppearance);
            }
            slimeTarr.icon = Get<Sprite>("iconSlimeTarr");
            identifiableTypeGroup.memberTypes.Add(slimeTarr); 
            nonSlimesGroup.memberTypes.Add(slimeTarr);
        }
        ColorUtility.TryParseHtmlString("#75d9ff", out var potColor);
        var nonLiquids = Get<IdentifiableTypeGroup>("VaccableNonLiquids");
        var localizedString = LocalizationUtil.CreateByKey("Actor", "l.container_case");
        foreach (var identType in Resources.FindObjectsOfTypeAll<IdentifiableType>().Where(x => x.prefab != null))
        {
            if (identType.prefab.name.StartsWith("container"))
            {
                identType.prefab.GetComponent<Vacuumable>().size = Vacuumable.Size.NORMAL;
                identType.icon = iconContainer;
                identType.color = potColor;
                identType.localizedName = localizedString;
                nonLiquids.memberTypes.Add(identType);

            }
            
        }
        foreach (var identifiableType in new Il2CppSystem.Collections.Generic.List<IdentifiableType>(largoGroup.GetAllMembers()))
        {
            
            var type = identifiableType.TryCast<SlimeDefinition>();
            if (type == null)
                continue;
            if (type.AppearancesDefault == null)
            {
                LateActivation.Add(type);
                continue;
            }
            if (type.referenceId == null)
                continue;
            if (type.prefab != null)
                type.prefab.GetComponent<Vacuumable>().size = Vacuumable.Size.NORMAL;
            SetLargoIconAndPalette(type);
            
        }
        nonLiquids.memberGroups.Add(largoGroup);
        nonSlimesGroup.memberGroups.Add(largoGroup);

    }
    public static void SetLargoIconAndPalette(SlimeDefinition type)
    {
      
        foreach (var slimeAppearance in type.AppearancesDefault)
        {
            var splatColor = AverageColorFromArray(type.BaseSlimes[0].AppearancesDefault[0].SplatColor, type.BaseSlimes[1].AppearancesDefault[0].SplatColor);
            type.color = splatColor;
            var colorPalette = slimeAppearance.ColorPalette;
            colorPalette.Ammo = splatColor;
            slimeAppearance._icon = iconLargoPedia;
            slimeAppearance._colorPalette = colorPalette; 
            type.icon = iconLargoPedia;
        }
           
    }
    public static Color32 AverageColorFromArray(params Color32[] color32)
    {

        int total = color32.Length;
        float r = 0;
        float g = 0;
        float b = 0;
        for (int i = 0; i < total; i++)
        {

            r += color32[i].r;

            g += color32[i].g;

            b += color32[i].b;

        }
        var averageColorFromArray = new Color32((byte)(r / total), (byte)(g / total), (byte)(b / total), 255);
        return averageColorFromArray;

    }

    public static T Get<T>(string name) where T : Object
    {
        return Resources.FindObjectsOfTypeAll<T>().FirstOrDefault(x => x.name == name);
    }

    public static Sprite ConvertSprite(Texture2D texture2D)
    {
        return Sprite.Create(texture2D, new Rect(0.0f, 0.0f, texture2D.width, texture2D.height),
            new Vector2(0.5f, 0.5f), 100.0f);
    }

    public Texture2D LoadImage(string image)
    {
        var manifestResourceStream = MelonAssembly.Assembly.GetManifestResourceStream(image);
        var bytes = new byte[manifestResourceStream.Length];
        _ = manifestResourceStream.Read(bytes);
        
        var texture2D = new Texture2D(1, 1);
        ImageConversion.LoadImage(texture2D, bytes);
        return texture2D;

    }
}