using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using XEditor.CustomSearchWindow;
using XPlayer.Input.InputSetting;
using UnityEditor.Experimental.GraphView;
using System;
using System.IO;

namespace XPlayer.Input.InputManager
{

    [CustomEditor(typeof(XInput))]
    public class XInputEditor : Editor
    {
        private static float lineHeight = EditorGUIUtility.singleLineHeight;

        private XInput xInput;
        private SerializedProperty inputSettings, index;
        private ReorderableList inputSettingList;

        [MenuItem("Assets/Open XInput")]
        public static void OpenInspector()
        {
            Selection.activeObject = XInput.Instance;
        }

        private void OnEnable()
        {
            xInput = target as XInput;
            inputSettings = serializedObject.FindProperty(XInput.PlayerInputSettings_Prop_Name);
            index = serializedObject.FindProperty(XInput.PresentIndex_Prop_Name);

            inputSettingList = new ReorderableList(serializedObject, inputSettings);
            inputSettingList.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "Player Input Settings");
            };
            inputSettingList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = inputSettings.GetArrayElementAtIndex(index).FindPropertyRelative(InputSetting.InputSetting.InputSettingName_Prop_Name);
                float length = EditorGUIUtility.currentViewWidth - 65;
                rect.height = lineHeight;
                rect.y += 3;
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, length-rect.x-5, rect.height), element, new GUIContent());
                EditorGUI.EndDisabledGroup();
                if(GUI.Button(new Rect(length, rect.y, 55, rect.height), "Open")) { InputSettingWindow.ShowWindow(xInput[index]); }
            };
            inputSettingList.onAddCallback = (list) =>
            {
                inputSettings.arraySize++;
                list.index = inputSettings.arraySize - 1;
                var element = inputSettings.GetArrayElementAtIndex(list.index).FindPropertyRelative(InputSetting.InputSetting.InputSettingName_Prop_Name);
                element.stringValue = String.Format("New Input Setting ({0})", list.index + 1);
            };
            inputSettingList.onRemoveCallback = (list) =>
            {
                inputSettings.DeleteArrayElementAtIndex(list.index);
                if (list.index >= inputSettings.arraySize)
                {
                    list.index = inputSettings.arraySize - 1;
                }
            };
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Label("Player Input Setting", EditorStyles.boldLabel);
            GUILayout.Space(5);
            EditorGUI.BeginDisabledGroup(true);
            xInput.PresentIndex = EditorGUILayout.IntField("Index", xInput.PresentIndex);
            EditorGUI.EndDisabledGroup();
            GUILayout.Space(5);

            inputSettingList.DoLayoutList();

            serializedObject.Update();

            /*
            if (GUILayout.Button($"{Path.GetFileName(AssetDatabase.GetAssetPath(xInput.PlayerInputSetting))}", EditorStyles.popup))
            {
                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), new ObjectSearchProvider(typeof(InputSetting.InputSetting), inputSetting));
            }
            */
        }
    }
}
