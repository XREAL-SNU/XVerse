using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XEditor.CustomSearchWindow;
using XPlayer.Input.InputSetting;
using UnityEditor.Experimental.GraphView;
using System;

namespace XPlayer.Input.InputManager
{

    [CustomEditor(typeof(XInput))]
    public class XInputEditor : Editor
    {
        XInput xInput;
        SerializedProperty inputSetting;

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
            EditorGUILayout.LabelField("Input Setting", GUILayout.Width(100));
            
            if (GUILayout.Button($"{xInput.PlayerInputSetting.InputSettingName}", EditorStyles.popup))
            {
                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), new ObjectSearchProvider(typeof(InputSetting.InputSetting), inputSetting));
            }

            GUILayout.EndHorizontal();
        }
    }
}
