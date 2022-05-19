using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XPalette;

namespace XAppearance.Property
{
    [System.Serializable]
    public class ObjectPartProperty
    {
        public XColorPalette ColorPalette;
        public int Pick;

        public ObjectPartProperty(XColorPalette palette)
        {
            if (palette != null && palette.ColorSet.Count > 0)
            {
                ColorPalette = palette;
                Pick = 0;
            }
            else
            {
                Debug.LogError($"{palette.PaletteName} palette has no ColorSet");
                return;
            }
        }

        public ObjectPartProperty(XColorPalette palette, int pick) : this(palette)
        {
            Pick = pick;
        }

        public ObjectPartProperty()
        {
            ColorPalette = null;
            Pick = 0;
        }

        public void CopyFrom(ObjectPartProperty source)
        {
            ColorPalette = source.ColorPalette;
            Pick = source.Pick;
        }

        public ObjectPartProperty SetProperty(XColorPalette palette, int pick)
        {
            ColorPalette = palette;
            Pick = pick;
            return this;
        }
    }
}