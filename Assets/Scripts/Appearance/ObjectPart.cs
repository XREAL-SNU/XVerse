using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XAppearance.Property
{
    [System.Serializable]
    public class ObjectPart
    {
        public string PartName;
        public string PartPath;
        public ObjectPartProperty Property;
        public static string PartName_Prop_Name => nameof(PartName);
        public static string PartPath_Prop_Name => nameof(PartPath);
        public static string Property_Prop_Name => nameof(Property);

        public ObjectPart(string name, string path)
        {
            PartName = name;
            PartPath = path;
            Property = new ObjectPartProperty();
        }

        public ObjectPart(string name, string path, ObjectPartProperty property) : this(name, path)
        {
            Property = property;
        }

        public ObjectPart() : this("Default", "Default") { }

        public void CopyFrom(ObjectPart part)
        {

        }
    }

}