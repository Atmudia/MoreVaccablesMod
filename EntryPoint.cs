global using Il2Cpp;
global using static MoreVaccablesMod.EntryPoint;
using System.Collections.Generic;
using System.Linq;
using MelonLoader;
using MoreVaccablesMod;
using MoreVaccablesMod.Patches;
using UnityEngine;
using Object = UnityEngine.Object;

[assembly: MelonInfo(typeof(EntryPoint), "MoreVaccablesMod", "1.3.4", "Atmudia", "https://www.nexusmods.com/slimerancher2/mods/42")]
[assembly: MelonGame("MonomiPark", "SlimeRancher2")]
namespace MoreVaccablesMod;

public class EntryPoint : MelonMod
{
    internal static IdentifiableTypeGroup LargoGroup;
    internal static IdentifiableTypeGroup VaccableBaseSlimeGroup;
    internal static IdentifiableTypeGroup NonSlimesGroup;
    internal static Sprite IconLargoPedia;
    internal static Sprite IconContainer;
    private MelonPreferences_Category _moreVaccablesMod;
    internal static MelonPreferences_Entry<bool> IsTarrEnabled;
    internal static MelonPreferences_Entry<bool> IsToysEnabled;
    internal static MelonPreferences_Entry<bool> IsSlimeFleeingDisabled;
    
    public static List<SlimeDefinition> LateActivation = [];
    public override void OnInitializeMelon()
    {
     
        _moreVaccablesMod = MelonPreferences.CreateCategory(nameof(_moreVaccablesMod));
        IsTarrEnabled = _moreVaccablesMod.CreateEntry("isTarrEnabled", true, "Is Tarr Enabled", "Should More Vaccable be able to vac Tarr Slime?");
        IsToysEnabled = _moreVaccablesMod.CreateEntry("isToysEnabled", true, "Are Toys Enabled", "Should More Vaccable be able to vac Toys?");
        IsSlimeFleeingDisabled = _moreVaccablesMod.CreateEntry("isSlimeFleeingDisabled", false, "Is Slime Fleeing Disabled", "Should gold, lucky and shadow flee?");
        MelonPreferences.Save();
        
        IconContainer ??= ConvertSprite(LoadImage("MoreVaccablesMod.iconContainer.png"));
        IconContainer.hideFlags |= HideFlags.HideAndDontSave;
        foreach (var methodInfo in typeof(IdentifiableType).GetMethods())
        {
            if (methodInfo.Name.Contains("TryGetId") && methodInfo.ReturnType == typeof(IdentifiableType))
            {
                Patch_VacuumItem.IdentifiableTypeTryGetId = methodInfo;
            }
        }

        if (Patch_VacuumItem.IdentifiableTypeTryGetId == null)
        {
            Melon<EntryPoint>.Logger.Msg("Method named IdentifiableType TryGetId is null, please report this issue on mod website");
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
            var splatColor = AverageColorFromArray(type.BaseSlimes[0]!.AppearancesDefault[0]!.SplatColor, type.BaseSlimes[1]!.AppearancesDefault[0]!.SplatColor);
            type.color = splatColor;
            var colorPalette = slimeAppearance.ColorPalette;
            colorPalette.Ammo = splatColor;
            slimeAppearance._icon = IconLargoPedia;
            slimeAppearance._colorPalette = colorPalette; 
            type.icon = IconLargoPedia;
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
        if (manifestResourceStream != null)
        {
            var bytes = new byte[manifestResourceStream.Length];
            _ = manifestResourceStream.Read(bytes);
        
            var texture2D = new Texture2D(1, 1);
            ImageConversion.LoadImage(texture2D, bytes);
            return texture2D;
        }

        return null;
    }
}
