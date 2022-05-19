using System.Collections.Generic;
using UnityEngine;
using System;

namespace XPalette
{
    [CreateAssetMenu(fileName = "NewPalette", menuName = "XAppearance/ColorPalette")]
    [Serializable]
    public class XColorPalette : ScriptableObject
    {
        public string PaletteName;
        public List<XColor> ColorSet;

        public static string ColorSet_Prop_Name => nameof(ColorSet);

        public XColor this[int index]
        {
            get
            {
                if(index < ColorSet.Count) { return ColorSet[index]; }
                else
                {
                    Debug.LogError("Palette/ requested index outside range");
                    return null;
                }
            }
        }

        public XColorPalette(string name)
        {
            PaletteName = name;
            ColorSet = new List<XColor>();
        }

        public XColorPalette(string name, List<XColor> colors)
        {
            PaletteName = name;
            ColorSet = colors;
        }

        public XColorPalette()
        {
            PaletteName = "Default";
            ColorSet = new List<XColor>();
        }
    }
}