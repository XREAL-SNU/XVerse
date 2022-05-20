using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XAppearance.Property;

namespace XAppearance
{
    [AddComponentMenu("XAppearance")]
    [RequireComponent(typeof(Rigidbody))]
    [System.Serializable]
    public class Appearance : MonoBehaviour
    {
        public ObjectPartsInfo objectPartsInfo;
        public List<GameObject> objectParts = new List<GameObject>();
        public static string objectParts_Prop_Name => nameof(objectPartsInfo);

        public GameObject GetObjectByPath(string path, GameObject target)
        {
            GameObject obj = target;
            if (target.transform.Find(path) != null)
            {
                obj = target.transform.Find(path).gameObject;
            }
            return obj;
        }

        public string GetPathByObject(GameObject obj)
        {
            string path = obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path += "/" + obj.name;
            }
            if (path.Split("/").Length > 1)
            {
                string[] strings = path.Split("/");
                for(int i = strings.Length - 2; i >= 0; i--)
                {
                    if(i == strings.Length - 2) { path = strings[i]; }
                    else { path += "/" + strings[i]; }
                }
            }
            return path;
        }
    }
}
