global using Il2Cpp;
global using static MoreVaccablesMod.EntryPoint;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.AdditionalContent.Epic;
using Il2CppMonomiPark.SlimeRancher.Platform.AdditionalContent;
using Il2CppMonomiPark.SlimeRancher.Player.PlayerItems;
using Il2CppMonomiPark.SlimeRancher.Script.Util;
using Il2CppMonomiPark.SlimeRancher.UI.Localization;
using MelonLoader;
using MoreVaccablesMod;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceProviders;
using UnityEngine.ResourceManagement.ResourceProviders;
using Object = UnityEngine.Object;
[assembly: MelonInfo(typeof(EntryPoint), "MoreVaccablesMod", "1.0.7", "KomiksPL", "https://www.nexusmods.com/slimerancher2/mods/42")]
namespace MoreVaccablesMod;

public class EntryPoint : MelonMod
{
    internal static IdentifiableTypeGroup largoGroup;
    internal static IdentifiableTypeGroup nonSlimesGroup;
    internal static Sprite iconLargoPedia;
    internal static Sprite iconContainer;
    private MelonPreferences_Category MoreVaccablesMod;
    internal static MelonPreferences_Entry<bool> isTarrEnabled;
    internal static MelonPreferences_Entry<bool> isToysEnabled;

    public static List<SlimeDefinition> LateActivation = [];

    public override void OnInitializeMelon()
    {
        MoreVaccablesMod = MelonPreferences.CreateCategory(nameof(MoreVaccablesMod));
        isTarrEnabled = MoreVaccablesMod.CreateEntry<bool>("isTarrEnabled", true, "Is Tarr Enabled", "Should More Vaccable be able to vac Tarr Slime?");
        isToysEnabled = MoreVaccablesMod.CreateEntry<bool>("isToysEnabled", true, "Is Toys Enabled", "Should More Vaccable be able to vac Toys?");
        iconContainer ??= ConvertSprite(LoadImage("MoreVaccablesMod.iconContainer.png"));
        
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
                    identifiableType.prefab.GetComponent<Vacuumable>().size = VacuumableSize.NORMAL;
                SetLargoIconAndPalette(identifiableType);
            }
        }
        
        if (!sceneName.Equals("GameCore")) return;
        
        
        
        var identifiableTypeGroup = Get<IdentifiableTypeGroup>("VaccableBaseSlimeGroup");
        nonSlimesGroup._memberGroups.Add(identifiableTypeGroup);
        SlimeDefinition slimeGold = Get<SlimeDefinition>("Gold");
        slimeGold.prefab.GetComponent<Vacuumable>().size = VacuumableSize.NORMAL;
        foreach (var slimeAppearance in slimeGold.AppearancesDefault)
            slimeGold.SetPalette(slimeAppearance);
        if (slimeGold.prefab.TryGetComponentButBetter<GoldSlimeFlee>(out var goldSlimeFlee))
            Object.Destroy(goldSlimeFlee);
        identifiableTypeGroup._memberTypes.Add(slimeGold);
        nonSlimesGroup._memberTypes.Add(slimeGold);

        SlimeDefinition slimeLucky = Get<SlimeDefinition>("Lucky");
        foreach (var slimeAppearance in slimeLucky.AppearancesDefault)
            slimeLucky.SetPalette(slimeAppearance);
        if (slimeLucky.prefab.TryGetComponentButBetter<LuckySlimeFlee>(out var slimeLuckyFlee))
            Object.Destroy(slimeLuckyFlee);
        identifiableTypeGroup._memberTypes.Add(slimeLucky);
        nonSlimesGroup._memberTypes.Add(slimeLucky);
        if (isTarrEnabled.Value)
        {
            SlimeDefinition slimeTarr = Get<SlimeDefinition>("Tarr");
            slimeTarr.prefab.GetComponent<Vacuumable>().size = VacuumableSize.NORMAL;
            foreach (var slimeAppearance in slimeTarr.AppearancesDefault)
            {
                if (slimeAppearance.SaveSet == SlimeAppearance.AppearanceSaveSet.CLASSIC)
                {
                    slimeAppearance._icon = Get<Sprite>("iconSlimeTarr");
                }
                slimeTarr.SetPalette(slimeAppearance);
            }
            slimeTarr.icon = Get<Sprite>("iconSlimeTarr");
            identifiableTypeGroup._memberTypes.Add(slimeTarr); 
            nonSlimesGroup._memberTypes.Add(slimeTarr);
        }
        ColorUtility.TryParseHtmlString("#75d9ff", out var potColor);
        var nonLiquids = Get<IdentifiableTypeGroup>("VaccableNonLiquids");
        var localizedString = LocalizationUtil.CreateByKey("Actor", "l.container_case");
        foreach (var identType in Resources.FindObjectsOfTypeAll<IdentifiableType>().Where(x => x.prefab != null))
        {
            if (identType.prefab.name.StartsWith("container"))
            {
                identType.prefab.GetComponent<Vacuumable>().size = VacuumableSize.NORMAL;
                identType.icon = iconContainer;
                identType.color = potColor;
                identType.localizedName = localizedString;
                nonLiquids._memberTypes.Add(identType);

            }
            
        }
        foreach (var identifiableType in new Il2CppSystem.Collections.Generic.List<IdentifiableType>(largoGroup.GetAllMembers()))
        {
            if (identifiableType.prefab != null)
                identifiableType.prefab.GetComponent<Vacuumable>().size = VacuumableSize.NORMAL;
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
            SetLargoIconAndPalette(type);
            // GameObject.CreatePrimitive(PrimitiveType.Sphere); 
            
        }

        if (isToysEnabled.Value)
        {
            var toyGroup = Get<IdentifiableTypeGroup>("ToyGroup");
            foreach (var identifiableTypePediaObject in Resources.FindObjectsOfTypeAll<AdditionalContentCatalog>().Where(x => x.Toys != null &&  x.Toys.Asset != null).SelectMany(x => x.Toys.Asset.Cast<IdentifiableTypePediaLinkMap>().IdentifiableTypePediaObjectMap.ToArray()))
            {
                toyGroup._memberTypes.Add(identifiableTypePediaObject.IdentifiableType);

            }
            toyGroup._runtimeObject = null;
            toyGroup.GetRuntimeObject();
            foreach (var toyId in toyGroup._memberTypes )
            {
                if (toyId.prefab != null)
                    toyId.prefab.GetComponent<Vacuumable>().size = VacuumableSize.NORMAL;
                if (toyId.icon != null)
                    SetPalette(toyId);
            }
        }

    }

    public static void SetPalette(IdentifiableType type)
    {
        var colorAverage = AverageColorFromTexture(type.icon.texture);
        type.color = colorAverage;
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
    public static Color32 AverageColorFromTexture(Texture2D tex)
    {
        // Create a RenderTexture
        RenderTexture renderTex = RenderTexture.GetTemporary(
            tex.width,
            tex.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear);

        // Blit the texture to the RenderTexture
        Graphics.Blit(tex, renderTex);

        // Create a new Texture2D and read the RenderTexture
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;

        Texture2D readableTex = new Texture2D(tex.width, tex.height);
        readableTex.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableTex.Apply();

        // Restore the previous RenderTexture
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);

        // Get the pixel colors
        Color32[] texColors = readableTex.GetPixels32();
        int total = texColors.Length;

        float r = 0;
        float g = 0;
        float b = 0;

        for (int i = 0; i < total; i++)
        {
            r += texColors[i].r;
            g += texColors[i].g;
            b += texColors[i].b;
        }

        // Clean up the temporary Texture2D
        Object.Destroy(readableTex);

        return new Color32((byte)(r / total), (byte)(g / total), (byte)(b / total), 255);
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