using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.Experimental.GraphView;

namespace XEditor.CustomSearchWindow
{
    [CustomEditor(typeof(SearchObjectAttribute))]
    public class SearchObjectAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.width -= 60;
            EditorGUI.ObjectField(position, property, label);

            position.x += position.width;
            position.width = 60;
            if(GUI.Button(position, new GUIContent("Find")))
            {
                Type t = (attribute as SearchObjectAttribute).searchObjectType;

                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), new ObjectSearchProvider(t, property));

            }
        }
    }
}
