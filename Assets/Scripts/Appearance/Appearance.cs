using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XAppearance.Property;

namespace XAppearance
{
    [RequireComponent(typeof(Rigidbody))]
    [System.Serializable]
    public class Appearance : MonoBehaviour
    {
        public ObjectParts objectParts;
        public Dictionary<string, GameObject> objectAppearance = new Dictionary<string, GameObject>();


    }
}
