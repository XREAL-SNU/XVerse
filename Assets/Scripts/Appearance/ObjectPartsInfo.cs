using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XAppearance.Property
{
    [System.Serializable]
    public class ObjectPartsInfo
    {
        public string ObjectType;
        public string ObjectName;
        public List<ObjectPart> Parts;
        public static string Parts_Prop_Name => nameof(Parts);

        public ObjectPartsInfo(string type, string name)
        {
            ObjectType = type;
            ObjectName = name;
            Parts = new List<ObjectPart>();
        }

        public ObjectPartsInfo(string type, string name, List<ObjectPart> parts) : this(type, name)
        {
            Parts = parts;
        }

        public ObjectPartsInfo() : this("Default", "Default") { }

        public ObjectPart this[string name]
        {
            get
            {
                ObjectPart part = null;
                for (int i = 0; i < Parts.Count; i++)
                {
                    if (Parts[i].PartName.Equals(name))
                    {
                        part = Parts[i];
                    }
                }
                if(part is null)
                {
                    Debug.LogError($"{ObjectName} doesn't have part wit name {name}");
                }
                return part;
            }
        }
    }

}