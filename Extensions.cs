using System.Linq;
using UnityEngine;

namespace MoreVaccablesMod;

public static class Extensions
{
    public static bool IsNull(this object @object) => @object == null;
    public static void SetPalette(this SlimeDefinition slimeDefinition, SlimeAppearance slimeAppearance)
    {
        var splatColor = slimeAppearance.SplatColor;
        var colorPalette = slimeAppearance.ColorPalette;
        colorPalette.Ammo = splatColor;
        slimeDefinition.color = splatColor;
        slimeAppearance._colorPalette = colorPalette;
    }

    public static bool TryGetComponentButWorking<T>(this GameObject @this, out T monoBehaviour) where T : MonoBehaviour
    {
        var component = @this.GetComponent<T>();
        if (component)
        {
            monoBehaviour = component;
            return true;
        }

        monoBehaviour = null;
        return false;
    }
}