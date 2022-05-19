using UnityEngine;

namespace XPalette
{
    [System.Serializable]
    public class XColor
    {
        public string ColorName;
        [Range(0f, 1f)]
        public float r;
        [Range(0f, 1f)]
        public float g;
        [Range(0f, 1f)]
        public float b;
        [Range(0f, 1f)]
        public float a;


        public XColor(string name, float r, float g, float b, float a)
        {
            ColorName = name;
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public XColor(string name, UnityEngine.Color col)
        {
            ColorName = name;
            r = col.r;
            g = col.g;
            b = col.b;
            a = col.a;
        }

        public XColor()
        {
            ColorName = "Default";
            r = 0f;
            g = 0f;
            b = 0f;
            a = 0f;
        }

        public UnityEngine.Color ToColor()
        {
            return new UnityEngine.Color(r, g, b, a);
        }
        public UnityEngine.Color ToColor(float a)
        {
            this.a = a;
            return new UnityEngine.Color(r, g, b, a);
        }
    }
}
