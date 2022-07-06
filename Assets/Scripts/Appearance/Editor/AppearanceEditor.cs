using System.Collections;
using System.Collections.Generic;
using XAppearance.Property;
using XPalette;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using XEditor.CustomSearchWindow;
using UnityEditor.Experimental.GraphView;
using System.IO;

namespace XAppearance
{
    [CustomEditor(typeof(Appearance))]
    public class AppearanceEditor : Editor
    {
        private Appearance appearance;
        private SerializedProperty matchedGameObjects, objectPartsInfo, objectParts;

        private ReorderableList reorderableList;

        private static float lineHeight = EditorGUIUtility.singleLineHeight;
        private static float lineSpace = lineHeight + 5;
        private static char[] folderSplit = { '/', '\\' };

        private void OnEnable()
        {
            appearance = target as Appearance;
            matchedGameObjects = serializedObject.FindProperty(Appearance.matchedGameObjects_Prop_Name);
            objectPartsInfo = serializedObject.FindProperty(Appearance.objectPartsInfo_Prop_Name);
            objectParts = objectPartsInfo.FindPropertyRelative(ObjectPartsInfo.Parts_Prop_Name);

            reorderableList = new ReorderableList(serializedObject, objectParts);
            reorderableList.draggable = false;
            reorderableList.elementHeight = lineSpace * 5 + 5;

            reorderableList.drawHeaderCallback = (rect) =>
            {
                GUI.Label(rect, "Appearance Info", EditorStyles.boldLabel);
            };

            reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var partName = objectParts.GetArrayElementAtIndex(index).FindPropertyRelative(ObjectPart.PartName_Prop_Name);
                var partPath = objectParts.GetArrayElementAtIndex(index).FindPropertyRelative(ObjectPart.PartPath_Prop_Name);
                var matchedGameObject = matchedGameObjects.GetArrayElementAtIndex(index);
                var colorPalette = objectParts.GetArrayElementAtIndex(index).FindPropertyRelative(ObjectPart.Property_Prop_Name).FindPropertyRelative(ObjectPartProperty.ColorPalette_Prop_Name);

                EditorGUI.PropertyField(new Rect(rect.x, rect.y + 5, rect.width, lineHeight), partName, new GUIContent("Part Name"));
                EditorGUI.LabelField(new Rect(rect.x, rect.y + lineSpace + 5, 100, lineHeight), new GUIContent("Part Path"));
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(new Rect(rect.x + 100, rect.y + lineSpace + 5, 150, lineHeight), partPath, new GUIContent());
                if (EditorGUI.EndChangeCheck())
                {
                    GameObject obj = appearance.GetObjectByPath(partPath.stringValue, appearance.gameObject);
                    matchedGameObject.objectReferenceValue = obj;
                }
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(new Rect(rect.x + 260, rect.y + lineSpace + 5, rect.width - 260, lineHeight), matchedGameObject, new GUIContent());
                if (EditorGUI.EndChangeCheck())
                {
                    partPath.stringValue = appearance.GetPathByObject(matchedGameObject.objectReferenceValue as GameObject);
                }
                
                EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineSpace * 2 + 5, rect.width, lineHeight), colorPalette);

                /*
                 * if (GUI.Button(new Rect(rect.x, rect.y + lineSpace * 2 + 5, rect.width, lineHeight), $"{Path.GetFileName(AssetDatabase.GetAssetPath(colorPalette.objectReferenceValue))}", EditorStyles.popup))
                {
                    SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), new ObjectSearchProvider(typeof(XColorPalette), colorPalette));
                }
                */
            };

            reorderableList.onAddCallback = (list) =>
            {
                objectParts.arraySize++;
                matchedGameObjects.arraySize++;
                list.index = objectParts.arraySize - 1;
                var partName = objectParts.GetArrayElementAtIndex(list.index).FindPropertyRelative(ObjectPart.PartName_Prop_Name);
                var partPath = objectParts.GetArrayElementAtIndex(list.index).FindPropertyRelative(ObjectPart.PartPath_Prop_Name);
                var matchedGameObject = matchedGameObjects.GetArrayElementAtIndex(list.index);
                var colorPalette = objectParts.GetArrayElementAtIndex(list.index).FindPropertyRelative(ObjectPart.Property_Prop_Name).FindPropertyRelative(ObjectPartProperty.ColorPalette_Prop_Name);
                partName.stringValue = appearance.gameObject.name;
                partPath.stringValue = appearance.GetPathByObject(appearance.gameObject);
                matchedGameObject.objectReferenceValue = appearance.gameObject;
                colorPalette.objectReferenceValue = AssetDatabase.LoadAssetAtPath<XColorPalette>("Assets/Resources/Default/DefaultPalette.asset");
            };
            reorderableList.onRemoveCallback = (list) =>
            {
                matchedGameObjects.DeleteArrayElementAtIndex(list.index);
                objectParts.DeleteArrayElementAtIndex(list.index);
                if(list.index >= objectParts.arraySize)
                {
                    list.index = objectParts.arraySize - 1;
                }
            };
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            /*
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(objectPartsInfo.FindPropertyRelative(ObjectPartsInfo.ObjectType_Prop_Name), new GUIContent("Obaject Type"));
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(objectPartsInfo.FindPropertyRelative(ObjectPartsInfo.ObjectName_Prop_Name), new GUIContent("Obaject Name"));
            GUILayout.Space(5);

            serializedObject.Update();
            reorderableList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();*/
        }

        /*
        private void AddColorPalettePath(XColorPalette current, SerializedProperty property)
        {
            string[] stylesPath = GetAllPaths("Palette");
            string[] styles = new string[stylesPath.Length + 1];
            styles[0] = "None";
            int c = 0;

            for (int i = 0; i < stylesPath.Length; i++)
            {
                string path = stylesPath[i];
                int f = path.LastIndexOfAny(folderSplit) + 1;
                int l = path.IndexOf(".");
                styles[i + 1] = path.Substring(f, l - f);
                if (current != null && current.name == styles[i + 1]) c = i + 1;
            }
            int palette = EditorGUILayout.Popup(c, styles);

            if (palette < 1) property.objectReferenceValue = null;
            else
            {
                property.objectReferenceValue = EUtils.FromPath<XColorPalette>(stylesPath[palette - 1]);
            }
        }

        private string[] GetAllPaths(string name)
        {
            string[] guids = AssetDatabase.FindAssets("t:" + name);
            for (int i = 0; i < guids.Length; i++)
            {
                guids[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
            }

            return guids;
        }*/
    }
}
