using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.AnimatedValues;

public static class Utils
{
    public class ObjectIncludeIsHide<T>
    {
        public List<AnimBool> ShowFades;
        public List<T> Objects;

        public ObjectIncludeIsHide(List<T> objects)
        {
            Objects = objects;
            ShowFades = new List<AnimBool>();
            Initaillize();
        }

        public void Initaillize()
        {
            if(Objects != null && Objects.Count > 0)
            {
                for (int i = 0; i < Objects.Count; i++)
                {
                    ShowFades.Add(new AnimBool(false));
                }
            }
        }
    }
}
