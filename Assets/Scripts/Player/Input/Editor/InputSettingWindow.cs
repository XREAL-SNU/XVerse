using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace XPlayer.Input.InputSetting
{
    public class InputSettingWindow : EditorWindow
    {
        InputSetting inputSetting = null;

        public static void ShowWindow(InputSetting inputSetting)
        {
            var window = CreateInstance<InputSettingWindow>();
            window.ShowUtility();
            window.inputSetting = inputSetting;

            window.titleContent = new GUIContent(window.inputSetting.InputSettingName);
            window.minSize = new Vector2(100, 100);
        }


        private void OnGUI()
        {
            //if (!isValid) { EditorGUILayout.HelpBox("Input Setting is not valid.", MessageType.Warning); }
            inputSetting.InputSettingName = EditorGUILayout.TextField("Input Setting Name", inputSetting.InputSettingName);
            GUILayout.Space(5);
            GUILayout.Label("1. Keyboard Input Setting");
            GUILayout.Space(5);
            GUILayout.Label("2. Mouse Input Setting");
            GUILayout.Space(5);
        }
    }
}
