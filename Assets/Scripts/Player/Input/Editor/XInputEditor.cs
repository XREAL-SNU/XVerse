using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
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
        XInput xInput;
        SerializedProperty inputSetting;
        Editor editor;

        [MenuItem("Assets/Open XInput")]
        public static void OpenInspector()
        {
            Selection.activeObject = XInput.Instance;
        }

        private void OnEnable()
        {
            xInput = target as XInput;
            inputSetting = serializedObject.FindProperty("PlayerInputSetting");
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Player Input Setting", GUILayout.Width(150));
            
            if (GUILayout.Button($"{Path.GetFileName(AssetDatabase.GetAssetPath(xInput.PlayerInputSetting))}", EditorStyles.popup))
            {
                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), new ObjectSearchProvider(typeof(InputSetting.InputSetting), inputSetting));
            }

            GUILayout.EndHorizontal();

            editor = Editor.CreateEditor(xInput.PlayerInputSetting);
            editor.OnInspectorGUI();
        }
    }
}
