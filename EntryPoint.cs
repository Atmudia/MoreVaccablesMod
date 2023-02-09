using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.SlimeRancher.UI.Adapter.Ammo;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu.Model;
using Il2CppMonomiPark.UnitPropertySystem;
using Il2CppSony.NP;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.IO;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityExplorer;
using File = System.IO.File;
using Object = UnityEngine.Object;

namespace MoreVaccablesMod
{
    public static class Extensions
    {
        public static void Log(this object @object) => MelonLogger.Msg(@object.ToString());
        public static bool isNull(this object @object) => @object == null;

        public static bool TryGetComponentButBetter<T>(this GameObject @this, out T monoBehaviour) where T : MonoBehaviour
        {
            var component = @this.GetComponent<T>();
            if (component != null)
            {
                monoBehaviour = component;
                return true;
            }

            monoBehaviour = null;
            return false;
        }

        public static string GetStringForSaveSet(this SlimeAppearance.AppearanceSaveSet saveSet) => saveSet switch
        {
            SlimeAppearance.AppearanceSaveSet.NONE => "Classic",
            SlimeAppearance.AppearanceSaveSet.CLASSIC => "Classic",
            SlimeAppearance.AppearanceSaveSet.SECRET_STYLE => "SecretStyle",
            _ => null
        };
        
        
        

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

    [HarmonyPatch(typeof(AmmoSlotViewHolder), nameof(AmmoSlotViewHolder.Awake))]
    public static class AmmoSlotAdapterOnSlotChangedPatch
    {
        public static void Prefix(AmmoSlotViewHolder __instance)
        {
            
            InspectorManager.Inspect(__instance.gameObject);
            var ammoSlot = __instance.transform.Find("Ammo Slot");
            var icon = ammoSlot.transform.Find("Icon");
            
            var slime1 = Object.Instantiate(icon, ammoSlot.transform).GetComponent<RectTransform>();
            slime1.name = "FirstSlime";
            slime1.sizeDelta /= 1.6f;
            slime1.anchoredPosition = new Vector2(9, 35);
            var slime2 = Object.Instantiate(icon, ammoSlot.transform).GetComponent<RectTransform>();
            slime2.name = "SecondSlime";
            slime2.sizeDelta /= 1.6f;
            slime2.anchoredPosition = new Vector2(-9, -9);
            
            slime1.gameObject.SetActive(false);
            slime2.gameObject.SetActive(false);
            
            
            //icon.gameObject.SetActive(false);
            


        }
    }

    [HarmonyPatch(typeof(VacColorAnimator), nameof(VacColorAnimator.Update))]
    public static class VacColorAnimatorUpdatePatch
    {
        public static GameObject AmmoSlots;
        public static Sprite iconLargoIcon;
        
        public static bool Prefix(VacColorAnimator __instance)
        {
            float num = __instance.colorTarget;
            float num2 = __instance.colorVal;
            if (num2 <= num)
            {
                if (num <= num2)
                {
                    goto IL_2F;
                }
                float deltaTime = Time.deltaTime;
            }
            float deltaTime2 = Time.deltaTime;
            __instance.colorVal = num;

            IL_2F:
            Ammo ammo = __instance.playerState.Ammo;
            GameObject selectedStored = ammo.GetSelectedStored();
            if (selectedStored is null)
                return true;
            EntryPoint.largoGroup ??= EntryPoint.Get<IdentifiableTypeGroup>("LargoGroup");
            iconLargoIcon ??= EntryPoint.largoSprites["iconLargoIcon"];
            var largoType = selectedStored.GetComponent<Identifiable>().identType;
            if (!EntryPoint.largoGroup.memberTypes.Contains(largoType))
                return true;
            if (largoType.icon != null)
                return true;


            int selectedAmmoIdx = ammo.selectedAmmoIdx;
            AmmoModel ammoModel = ammo.ammoModel;
            Ammo.Slot slot = ammoModel.slots[selectedAmmoIdx];
            var chosenSlimeAppearance = __instance.slimeAppearanceDirector.GetChosenSlimeAppearance(largoType).ColorPalette;
            int slotMaxCount = ammoModel.GetSlotMaxCount(slot.id, selectedAmmoIdx);
            Material material = __instance.vacSpiralMat;
            float value = __instance.colorVal;
            int property_SPIRAL_COLOR = VacColorAnimator.PROPERTY_SPIRAL_COLOR;
            material.SetFloat(property_SPIRAL_COLOR, value);
            Material material2 = __instance.vacDialMat;
            float value2 = __instance.colorVal;
            int property_SPIRAL_COLOR2 = VacColorAnimator.PROPERTY_SPIRAL_COLOR;
            material2.SetFloat(property_SPIRAL_COLOR2, value2);
            Material material3 = __instance.vacDialMat;
            int property_AMMO_FULLNESS = VacColorAnimator.PROPERTY_AMMO_FULLNESS;
            material3.SetFloat(property_AMMO_FULLNESS, 1f);
            Material material4 = __instance.vacDialMat;
            int property_AMMO_COLOR = VacColorAnimator.PROPERTY_AMMO_COLOR;
            material4.SetColor(property_AMMO_COLOR, slot.id.color);
            Material material5 = __instance.vacDialMat;
            int property_ICON = VacColorAnimator.PROPERTY_ICON;
            material5.SetTexture(property_ICON, iconLargoIcon.texture);

            return false;
            
        } 
          
    }

    
    [HarmonyPatch(typeof(AmmoSlotViewHolder), nameof(AmmoSlotViewHolder.UpdateAmmoDisplay))]
    public static class AmmoSlotViewHolderUpdateAmmoDisplay
    {
        public static Sprite largoIdentificator;
        public static bool Prefix(AmmoSlotViewHolder __instance, Ammo.Slot data)
        {
            if (largoIdentificator == null)
            {
                var largoIdentificatorTexture = new Texture2D(1, 1);
                largoIdentificatorTexture.hideFlags |= HideFlags.HideAndDontSave;
                largoIdentificator = EntryPoint.ConvertSprite(largoIdentificatorTexture);
                largoIdentificator.hideFlags |= HideFlags.HideAndDontSave;
                
                largoIdentificator.name = "LargoIdent";
            }
            try
            {
                if (!EntryPoint.largoGroup.memberTypes.Contains(data.id))
                    return true;
                MelonLogger.Msg("Chunk 1");
                if (data.id.icon != null)
                    return true;
     
                var slimeDefinition = data.id.Cast<SlimeDefinition>();
                if (slimeDefinition.BaseSlimes.Count != 2)
                    return true;
                bool isEmpty = data.count == 0;
                __instance.isEmpty = isEmpty;
                IdentifiableType id = data.id;
                int index = data.index;
                int slotMaxCount = SRSingleton<SceneContext>.Instance.PlayerState.Ammo.GetSlotMaxCount(id, index);

                var firstSlime = __instance.transform.Find("Ammo Slot/FirstSlime").gameObject;
                var secondSlime = __instance.transform.Find("Ammo Slot/SecondSlime").gameObject;

                if (isEmpty)
                {
                    firstSlime.SetActive(false);
                    secondSlime.SetActive(false);
                    var emptyImage = firstSlime.transform.parent.transform.Find("Icon").GetComponent<Image>();
                    emptyImage.gameObject.SetActive(true);
                    emptyImage.sprite = __instance.emptyIcon;
                    data.maxCountValue.property.cachedValue *= 2;
                    return false;
                }
                
                MelonLogger.Msg("Chunk 2");
     
                MelonLogger.Msg("Chunk 2");
                SlimeAppearance slimeAppearance = SRSingleton<SceneContext>.Instance.SlimeAppearanceDirector.GetChosenSlimeAppearance(data.id);
                data.appearance = slimeAppearance.SaveSet;
              

                data.maxCountValue.property.cachedValue = slotMaxCount;

                MelonLogger.Msg("Chunk 3");
            
                Image icon = __instance.icon;
                icon.sprite = largoIdentificator;
                icon.gameObject.SetActive(false);
                
                
                firstSlime.GetComponent<Image>().sprite = slimeDefinition.BaseSlimes[0].icon ? slimeDefinition.BaseSlimes[0].icon : __instance.emptyIcon;
                firstSlime.gameObject.SetActive(true);
                secondSlime.GetComponent<Image>().sprite = slimeDefinition.BaseSlimes[1].icon ? slimeDefinition.BaseSlimes[1].icon : __instance.emptyIcon;
                secondSlime.gameObject.SetActive(true);
                LocalizedString localizedName = id.localizedName;
                TMP_Text tmp_Text = __instance.label;
                var colorPalette = slimeAppearance.ColorPalette;
                var averageColorFromArray = EntryPoint.AverageColorFromArray(colorPalette.Bottom, colorPalette.Top, colorPalette.Middle);
                StatusBar statusBar = __instance.bar;
                statusBar.barColor = averageColorFromArray;
                statusBar.currValue = data.Count;
                statusBar.maxValue = slotMaxCount;
                MelonLogger.Msg(slotMaxCount);

                if (localizedName != null)
                {
                    tmp_Text.text = localizedName.GetLocalizedString();
                    tmp_Text.color = averageColorFromArray;
                }
                MelonLogger.Msg("Chunk 5");
                if (data.count == 0)
                {
                    __instance.hideNameplateAt = 0;
                    
                }
                __instance.ShowNameplate();
                
                InputActionReference hotkey = data.definition.hotkey;
                __instance.BindInputKey(hotkey);
                Sprite containerInnerOverride = data.definition.containerInnerOverride;
                if (containerInnerOverride == null)
                {
                    Image image2 = __instance.containerOuter;
                    image2.sprite = __instance.containerOuterDefault;
                    Image image3 = __instance.containerInner;
                    image3.sprite = __instance.containerInnerDefault;
                    Image image4 = __instance.containerFill;
                    image4.sprite = __instance.containerFillDefault;
                    Image image5 = __instance.containerFillBehind;
                    image5.sprite = __instance.containerFillDefault;
                    return false;
                }
                MelonLogger.Msg("Chunk 6");
                AmmoSlotDefinition definition = data.definition;
                Image image6 = __instance.containerOuter;
                image6.sprite = definition.containerShapeOverride;
                AmmoSlotDefinition definition2 = data.definition;
                Image image7 = __instance.containerInner;
                image7.sprite = definition2.containerInnerOverride;
                AmmoSlotDefinition definition3 = data.definition;
                Image image8 = __instance.containerFill;
                image8.sprite = definition3.containerFillOverride;
                MelonLogger.Msg("Chunk 7");





            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
           
            return false;
        }
    }
    
    

    [HarmonyPatch(typeof(AmmoModel), nameof(AmmoModel.SetSlotMaxCountFunction))]//, nameof(Ammo.GetSlotMaxCount), typeof(IdentifiableType), typeof(int))]
    public static class GetSlotMaxCountPatch
    {
        public static IdentifiableType tarrIdent;
        public static IdentifiableType goldIdent;
        public static IdentifiableType luckyIdent;

        public static void Prefix(Il2CppSystem.Func<IdentifiableType, int, int> slotMaxCountFunction)
        {
            slotMaxCountFunction += new System.Func<IdentifiableType, int, int>(delegate(IdentifiableType type, int i)
            {
                return 0;
            });
        }
    }
    [HarmonyPatch(typeof(SlimeAppearanceDirector), nameof(SlimeAppearanceDirector.GetSpecificSlimeAppearance))]
    public static class GetChosenSlimeAppearance
    {
        public static bool Prefix(SlimeAppearanceDirector __instance, IdentifiableType slimeId, SlimeAppearance.AppearanceSaveSet saveSet, ref SlimeAppearance __result)
        {
            EntryPoint.largoGroup ??= EntryPoint.Get<IdentifiableTypeGroup>("LargoGroup");
            if (!EntryPoint.largoGroup.IsMember(slimeId))
                return true;
            __result = __instance.GetChosenSlimeAppearance(slimeId);
            return false;
        }
    }

    public class EntryPoint : MelonMod
    {
        public static IdentifiableTypeGroup largoGroup;
        public static Dictionary<string, Sprite> largoSprites = new Dictionary<string, Sprite>();

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

            var averageColorFromArray = new Color32((byte)(r / total) , (byte)(g / total) , (byte)(b / total) , 255);
            averageColorFromArray.ToString().Log();
            return averageColorFromArray;

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
            AssetBundle assetBundle = AssetBundle.LoadFromFile(@"E:\SlimeRancherModding\Unity\MoreVaccables\Assets\AssetBundles\largoicons");
            foreach (var loadAllAsset in assetBundle.LoadAllAssets(Il2CppType.Of<Sprite>()))
            {
                var sprite = loadAllAsset.Cast<Sprite>();
                sprite.hideFlags |= HideFlags.HideAndDontSave;
                largoSprites.Add(loadAllAsset.name, sprite);
            }
            
            Action<Scene, LoadSceneMode> scene = (scene1, mode) =>
            {
                
                if (!scene1.name.Equals("GameCore")) return;
                EntryPoint.largoGroup ??= Get<IdentifiableTypeGroup>("LargoGroup");
                var identifiableTypeGroup = Get<IdentifiableTypeGroup>("VaccableBaseSlimeGroup");

                SlimeDefinition slimeGold = (SlimeDefinition)(GetSlotMaxCountPatch.goldIdent = Get<SlimeDefinition>("Gold"));
                slimeGold.prefab.GetComponent<Vacuumable>().size = Vacuumable.Size.NORMAL;

                foreach (var slimeAppearance in slimeGold.Cast<SlimeDefinition>().AppearancesDefault)
                {
                    var slimeAppearanceColorPalette = slimeAppearance.ColorPalette;
                    Color32 color = slimeAppearanceColorPalette.Top;
                    slimeAppearanceColorPalette.Ammo = color;
                    slimeGold.color = color;
                    slimeAppearance.ColorPalette = slimeAppearanceColorPalette;

                }
                if (slimeGold.prefab.TryGetComponentButBetter<GoldSlimeFlee>(out var goldSlimeFlee))
                    Object.Destroy(goldSlimeFlee);
                identifiableTypeGroup.memberTypes.Add(slimeGold);

                SlimeDefinition slimeLucky = (SlimeDefinition)(GetSlotMaxCountPatch.luckyIdent = Get<SlimeDefinition>("Lucky"));
                slimeLucky.prefab.GetComponent<Vacuumable>().size = Vacuumable.Size.NORMAL;
                foreach (var slimeAppearance in slimeLucky.Cast<SlimeDefinition>().AppearancesDefault)
                {
                    var slimeAppearanceColorPalette = slimeAppearance.ColorPalette;
                    Color32 color = slimeAppearanceColorPalette.Bottom;
                    slimeAppearanceColorPalette.Ammo = color;
                    slimeLucky.color = color;

                    slimeAppearance.ColorPalette = slimeAppearanceColorPalette;
                    

                }
                
    
                if (slimeLucky.prefab.TryGetComponentButBetter<LuckySlimeFlee>(out var slimeLuckyFlee))
                    Object.Destroy(slimeLuckyFlee);
                identifiableTypeGroup.memberTypes.Add(slimeLucky);

                SlimeDefinition tarrIdent = (SlimeDefinition)(GetSlotMaxCountPatch.tarrIdent = Get<SlimeDefinition>("Tarr"));
                tarrIdent.prefab.GetComponent<Vacuumable>().size = Vacuumable.Size.NORMAL;

                identifiableTypeGroup.memberTypes.Add(tarrIdent);
                foreach (var slimeAppearance in tarrIdent.AppearancesDefault)
                {
                    var slimeAppearanceColorPalette = slimeAppearance.ColorPalette;
                    Color32 color = slimeAppearanceColorPalette.Bottom;
                    slimeAppearanceColorPalette.Ammo = color;
                    tarrIdent.color = color;

                    slimeAppearance.ColorPalette = slimeAppearanceColorPalette;
                    

                }

                foreach (var identifiableType in largoGroup.memberTypes)
                {
                    
                    var type = identifiableType.TryCast<SlimeDefinition>(); 
                    if (type == null)
                        continue;
                    if (type.referenceId is null)
                        continue;
                    if (type.prefab != null)
                        type.prefab.GetComponent<Vacuumable>().size = Vacuumable.Size.NORMAL;
                    foreach (var slimeAppearance in type.AppearancesDefault)
                    {
                        var slimeAppearanceColorPalette = slimeAppearance.ColorPalette;
                        //slimeAppearance.name.Log();
                        if (!largoSprites.TryGetValue(slimeAppearance.name, out Sprite value))
                        {
                            type.color = AverageColorFromArray(slimeAppearanceColorPalette.Top, slimeAppearanceColorPalette.Middle, slimeAppearanceColorPalette.Bottom);
                            continue;
                        }
                        Color32 color = AverageColorFromArray(slimeAppearanceColorPalette.Top, slimeAppearanceColorPalette.Middle, slimeAppearanceColorPalette.Bottom);
                        if (slimeAppearance.SaveSet == SlimeAppearance.AppearanceSaveSet.NONE)
                        {
                            type.color = color;
                            identifiableType.icon = value;

                        }
                        slimeAppearanceColorPalette.Ammo = color;
                        slimeAppearance.Icon = value;
                        slimeAppearance.ColorPalette = slimeAppearanceColorPalette;
                        
                    }
                }
                Get<IdentifiableTypeGroup>("VaccableNonLiquids").memberGroups.Add(largoGroup);
            };
            SceneManager.add_sceneLoaded(scene);
        }

    }}
