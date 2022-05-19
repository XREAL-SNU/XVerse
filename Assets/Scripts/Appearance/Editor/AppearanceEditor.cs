using System.Collections;
using System.Collections.Generic;
using XAppearance.Property;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace XAppearance
{
    [CustomEditor(typeof(Appearance))]
    public class AppearanceEditor : Editor
    {
        private Appearance appearance;
        private SerializedProperty objectParts;

        private ReorderableList reorderableList;

        private static float lineHeight = EditorGUIUtility.singleLineHeight;
        private static float lineSpace = lineHeight + 5;

        private void OnEnable()
        {
            appearance = target as Appearance;
            objectParts = serializedObject.FindProperty(Appearance.objectParts_Prop_Name + "." + ObjectPartsInfo.Parts_Prop_Name);
            reorderableList = new ReorderableList(serializedObject, objectParts);
            reorderableList.draggable = false;

            reorderableList.drawHeaderCallback = DrawHeaderCallback;

            reorderableList.elementHeightCallback = ElementHeightCallback;
            reorderableList.drawElementCallback = DrawElementCallback;

            reorderableList.onAddCallback = OnAddCallback;
            reorderableList.onRemoveCallback = OnRemoveCallback;
        }

        private void DrawHeaderCallback(Rect rect)
        {
            GUI.Label(rect, "Appearance Info", EditorStyles.boldLabel);
        }

        private float ElementHeightCallback(int index)
        {
            int num = 4;
            return lineSpace * num + 5;
        }

        private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            EditorGUI.LabelField(new Rect(rect.x, rect.y + 5, rect.width, lineHeight), $"Part {index}", EditorStyles.boldLabel);
            appearance.objectPartsInfo.Parts[index].PartName = EditorGUI.TextField(new Rect(rect.x, rect.y + lineSpace + 5, rect.width, lineHeight), "Part Name", appearance.objectPartsInfo.Parts[index].PartName);
            EditorGUI.BeginChangeCheck();
            appearance.objectPartsInfo.Parts[index].PartPath = EditorGUI.TextField(new Rect(rect.x, rect.y + lineSpace * 2 + 5, rect.width, lineHeight), "Part Path", appearance.objectPartsInfo.Parts[index].PartPath);
            if (EditorGUI.EndChangeCheck())
            {
                GameObject obj = appearance.GetObjectByPath(appearance.objectPartsInfo.Parts[index].PartPath, appearance.gameObject);
                appearance.objectParts[index] = obj;
            }
            EditorGUI.BeginChangeCheck();
            appearance.objectParts[index] = (GameObject)EditorGUI.ObjectField(new Rect(rect.x, rect.y + lineSpace * 3 + 5, rect.width, lineHeight), appearance.objectParts[index], typeof(GameObject), true);
            if (EditorGUI.EndChangeCheck())
            {
                appearance.objectPartsInfo.Parts[index].PartPath = appearance.GetPathByObject(appearance.objectParts[index]);
            }
        }

        private void OnAddCallback(ReorderableList list)
        {
            appearance.objectPartsInfo.Parts.Add(new ObjectPart());
            objectParts = serializedObject.FindProperty(Appearance.objectParts_Prop_Name + "." + ObjectPartsInfo.Parts_Prop_Name);
            appearance.objectParts.Add(null);
        }

        private void OnRemoveCallback(ReorderableList list)
        {
            appearance.objectPartsInfo.Parts.RemoveAt(list.index);
            appearance.objectParts.RemoveAt(list.index);
            objectParts = serializedObject.FindProperty(Appearance.objectParts_Prop_Name + "." + ObjectPartsInfo.Parts_Prop_Name);
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            GUILayout.Space(5);
            appearance.objectPartsInfo.ObjectType = EditorGUILayout.TextField("Object Type", appearance.objectPartsInfo.ObjectType);
            GUILayout.Space(5);
            appearance.objectPartsInfo.ObjectName = EditorGUILayout.TextField("Object Name", appearance.objectPartsInfo.ObjectName);
            GUILayout.Space(5);

            serializedObject.Update();
            reorderableList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
