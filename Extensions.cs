using System.Linq;
using MelonLoader;
using UnityEngine;

namespace MoreVaccablesMod;

public static class Extensions
{
    public static void Log(this object @object) => MelonLogger.Msg(@object.ToString());
    public static bool IsNull(this object @object) => @object == null;
    public static void SetPalette(this SlimeDefinition slimeDefinition, SlimeAppearance slimeAppearance)
    {
        var splatColor = slimeAppearance.SplatColor;
        var colorPalette = slimeAppearance.ColorPalette;
        colorPalette.Ammo = splatColor;
        slimeDefinition.color = splatColor;
        slimeAppearance.ColorPalette = colorPalette;
    }

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
        text = param.Aggregate(text, (current, obj) => current + ", " + obj.ToString());

        MelonLogger.Msg(text);
        return text;
    }
}