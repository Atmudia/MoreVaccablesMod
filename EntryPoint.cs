using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.UnitPropertySystem;
using MelonLoader;
using UnityEngine;
using UnityEngine.IO;
using UnityEngine.SceneManagement;
using File = System.IO.File;
using Object = UnityEngine.Object;

namespace MoreVaccablesMod
{
    public static class MelonExtensions
    {
        public static void Log(this object @object) => MelonLogger.Msg(@object.ToString());
        public static bool isNull(this object @object) => @object == null;

        public static string LogMultiple(this object @this, params object[] param)
        {
            string text = @this.ToString();
            foreach (object obj in param)
            {
                text = text + ", " + obj.ToString();
            }

            MelonLogger.Msg(text);
            return text;
        }
    }

    [HarmonyPatch(typeof(Ammo), nameof(Ammo.GetSlotMaxCount), typeof(IdentifiableType), typeof(int))]
    public static class GetSlotMaxCount
    {
        public static void Postfix(IdentifiableType id, int index, ref int __result)
        {
            EntryPoint.largoGroup ??= EntryPoint.Get<IdentifiableTypeGroup>("LargoGroup");
            if (EntryPoint.largoGroup.memberTypes.Contains(id))
            {
                __result /= 2;
            }

        }
    }
    [HarmonyPatch(typeof(SlimeAppearanceDirector), nameof(SlimeAppearanceDirector.GetSpecificSlimeAppearance))]
    public static class GetChosenSlimeAppearance
    {
        public static bool Prefix(IdentifiableType slimeId, ref SlimeAppearance __result)
        {
            EntryPoint.largoGroup ??= EntryPoint.Get<IdentifiableTypeGroup>("LargoGroup");
            if (EntryPoint.largoGroup.memberTypes.Contains(slimeId))
            {
                __result = slimeId.Cast<SlimeDefinition>().AppearancesDefault[0];
                return false;
            }

            return true;
        }
    }

    public class EntryPoint : MelonMod
    {
        public static IdentifiableTypeGroup largoGroup;
        public static Sprite nonDestroyableImage;
        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            
        }

        public static Color32 AverageColorFromArray(params Color32[] color32)
        {
            
            int total = color32.Length;

            float r = 0;
            float g = 0;
            float b = 0;

            for(int i = 0; i < total; i++)
            {

                r += color32[i].r;

                g += color32[i].g;

                b += color32[i].b;

            }

            return new Color32((byte)(r / total) , (byte)(g / total) , (byte)(b / total) , 0);

        }
        public static T Get<T>(string name) where T : Object
        {
            return Resources.FindObjectsOfTypeAll<T>().FirstOrDefault(x => x.name == name);
        }
        public static Sprite ConvertSprite(Texture2D texture2D)
        {
            return Sprite.Create(texture2D, new Rect(0.0f, 0.0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 100.0f);
        }
        
        public override void OnInitializeMelon()
        {
            Action<Scene, LoadSceneMode> scene = (scene1, mode) =>
            {
                if (scene1.name != "GameCore") return;
                EntryPoint.largoGroup ??= Get<IdentifiableTypeGroup>("LargoGroup");
                var identifiableTypeGroup = Get<IdentifiableTypeGroup>("VaccableBaseSlimeGroup");

                var slimeGold = Get<IdentifiableType>("Gold");
                var goldSlimeFlee = slimeGold.prefab.GetComponent<GoldSlimeFlee>();
                if (goldSlimeFlee is not null)
                {
                    Object.Destroy(goldSlimeFlee);
                }
                identifiableTypeGroup.memberTypes.Add(slimeGold);

                var slimeLucky = Get<IdentifiableType>("Lucky");

                var luckySlimeFlee = slimeLucky.prefab.GetComponent<LuckySlimeFlee>();
                if (luckySlimeFlee is not null)
                {
                    Object.Destroy(luckySlimeFlee);
                }
                identifiableTypeGroup.memberTypes.Add(slimeLucky);
                if (nonDestroyableImage is null)
                {
                    Texture2D texture2D = new Texture2D(2, 2);
                    texture2D.hideFlags |= HideFlags.HideAndDontSave;
                    Il2CppImageConversionManager.LoadImage(texture2D, File.ReadAllBytes(@"C:\Users\KomiksPL\Desktop\conservatory_image.png"));
                    var convertSprite = ConvertSprite(texture2D);
                    nonDestroyableImage = convertSprite;
                    nonDestroyableImage.hideFlags |= HideFlags.HideAndDontSave;
                }

                foreach (var VARIABLE in largoGroup.memberTypes)
                {
                    if (VARIABLE.prefab != null)
                    {
                        VARIABLE.prefab.GetComponent<Vacuumable>().size = Vacuumable.Size.NORMAL;
                    }
                    var slimeDefinition = VARIABLE.TryCast<SlimeDefinition>();
                    slimeDefinition.AppearancesDefault[0].Icon = nonDestroyableImage;
                    VARIABLE.icon = nonDestroyableImage;
                    var colorPalette = slimeDefinition.AppearancesDefault[0].ColorPalette;
                    var averageColorFromArray = AverageColorFromArray(colorPalette.Top, colorPalette.Middle, colorPalette.Bottom);
                    Color color = averageColorFromArray;
                    color.a = 1;
                    VARIABLE.color = colorPalette.Ammo = color;
                    slimeDefinition.AppearancesDefault[0].ColorPalette = colorPalette;
                }
                Get<IdentifiableTypeGroup>("VaccableNonLiquids").memberGroups.Add(largoGroup);
            };
            SceneManager.add_sceneLoaded(scene);
        }

    }}
