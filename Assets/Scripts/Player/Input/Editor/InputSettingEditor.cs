using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using XPlayer.Input.Keyboard;
using XPlayer.Input.Mouse;
using UnityEngine.UIElements;

namespace XPlayer.Input.InputSetting
{
    [CustomEditor(typeof(InputSetting))]
    public class InputSettingEditor : Editor
    {
        private string[][] keyboardName = new string[][]
        {
            new string[]{"`", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "-", "+", "Back"},
            new string[]{"Tab", "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "[", "]", "|"},
            new string[]{"Caps", "A", "S", "D", "F","G", "H", "J", "K", "L", ";", "'", "Enter"},
            new string[]{"Shift", "Z", "X", "C", "V", "B", "N", "M", ",", ".", "/", "Shift"},
            new string[]{"Control", "Alt", "Space", "��", "��", "��", "��"}
        };

        private KeyCode[][] keyboardCode = new KeyCode[][]
        {
            new KeyCode[]{KeyCode.BackQuote, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5,
                KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0, KeyCode.Minus, KeyCode.Plus, KeyCode.Backspace},
            new KeyCode[]{KeyCode.Tab, KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.Y, KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P,
                KeyCode.LeftBracket, KeyCode.RightBracket, KeyCode.Backslash},
            new KeyCode[]{KeyCode.CapsLock, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L,
                KeyCode.Semicolon, KeyCode.Quote, KeyCode.Return},
            new KeyCode[]{KeyCode.LeftShift, KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.B, KeyCode.N, KeyCode.M,
                KeyCode.Comma, KeyCode.Period, KeyCode.Slash, KeyCode.RightShift},
            new KeyCode[]{KeyCode.LeftControl, KeyCode.LeftCommand, KeyCode.Space, KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow, KeyCode.RightArrow}
        };

        private int[][] keyboardSize = new int[][]
        {
            new int[]{1,1,1,1,1,1,1,1,1,1,1,1,1,2},
            new int[]{2,1,1,1,1,1,1,1,1,1,1,1,1,1},
            new int[]{2,1,1,1,1,1,1,1,1,1,1,1,2},
            new int[]{2,1,1,1,1,1,1,1,1,1,2,2},
            new int[]{2,2,7,1,1,1,1}
        };

        private const int keyboardCellSize = 30;
        private const int categoryWidth = 160;
        private const int categoryHeight = 150;
        private const string defaultToolTip = "None";
        private Dictionary<KeyCode, List<string>> keyCodeInputSetting;

        private Color unassignedKeyColor;
        private Color assignedKeyColor;

        private InputSetting inputSetting;

        private SerializedProperty keyboardGroups, keyboardInputs, mouseGroups, mouseInputs;

        private ReorderableList keyboardGroupList, keyboardInputList, mouseGroupList, mouseInputList;

        private static float lineHeight = EditorGUIUtility.singleLineHeight;
        private static float lineSpace = lineHeight + 5;

        private Vector2[] scrollPos;
        private bool isLock;
        private string isLockText;

        private void OnEnable()
        {
            inputSetting = target as InputSetting;
            keyboardGroups = serializedObject.FindProperty(InputSetting.KeyboardInputSetting_Prop_Name);
            mouseGroups = serializedObject.FindProperty(InputSetting.MouseInputSetting_Prop_Name);
            keyboardInputs = keyboardGroups.GetArrayElementAtIndex(0).FindPropertyRelative(KeyboardInputGroup.Inputs_Prop_Name);
            mouseInputs = mouseGroups.GetArrayElementAtIndex(0).FindPropertyRelative(MouseInputGroup.Inputs_Prop_Name);

            initializeAttribute();

            keyboardGroupList = new ReorderableList(serializedObject, keyboardGroups, true, false, false, false);
            keyboardInputList = new ReorderableList(serializedObject, keyboardInputs, true, false, false, false);

            keyboardGroupList.elementHeight = lineSpace;
            keyboardGroupList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = keyboardGroups.GetArrayElementAtIndex(index).FindPropertyRelative(KeyboardInputGroup.InputGroupName_Prop_Name);
                rect.height = lineHeight;
                rect.y += 3;
                EditorGUI.BeginDisabledGroup(isLock);
                EditorGUI.PropertyField(rect, element, new GUIContent());
                EditorGUI.EndDisabledGroup();
            };
            keyboardGroupList.onAddCallback = (list) =>
            {
                keyboardGroups.arraySize++;
                list.index = keyboardGroups.arraySize - 1;
                var element = keyboardGroups.GetArrayElementAtIndex(list.index).FindPropertyRelative(KeyboardInputGroup.InputGroupName_Prop_Name);
                element.stringValue = String.Format("New ({0})", list.index + 1);
            };
            keyboardGroupList.onRemoveCallback = (list) =>
            {
                keyboardGroups.DeleteArrayElementAtIndex(list.index);
                if (list.index >= keyboardGroups.arraySize)
                {
                    list.index = keyboardGroups.arraySize - 1;
                    keyboardInputs = keyboardGroups.GetArrayElementAtIndex(list.index).FindPropertyRelative(KeyboardInputGroup.Inputs_Prop_Name);
                    keyboardInputList.serializedProperty = keyboardInputs;
                }
            };
            keyboardGroupList.onCanRemoveCallback = (list) =>
            {
                return keyboardGroups.arraySize < 2 ? false : true;
            };
            keyboardGroupList.onSelectCallback = (list) =>
            {
                keyboardInputs = keyboardGroups.GetArrayElementAtIndex(list.index).FindPropertyRelative(KeyboardInputGroup.Inputs_Prop_Name);
                keyboardInputList.serializedProperty = keyboardInputs;
            };

            keyboardInputList.elementHeight = lineSpace;
            keyboardInputList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var command = keyboardInputs.GetArrayElementAtIndex(index).FindPropertyRelative(KeyboardInput.InputName_Prop_Name);
                var keytype = keyboardInputs.GetArrayElementAtIndex(index).FindPropertyRelative(KeyboardInput.InputKeyType_Prop_Name);
                var keycode = keyboardInputs.GetArrayElementAtIndex(index).FindPropertyRelative(KeyboardInput.InputKeyName_Prop_Name);
                rect.height = lineHeight;
                rect.width = 90;
                rect.y += 3;
                EditorGUI.BeginDisabledGroup(isLock);
                EditorGUI.PropertyField(rect, command, new GUIContent());
                rect.x += 95;
                EditorGUI.PropertyField(rect, keytype, new GUIContent());
                rect.x += 95;
                EditorGUI.PropertyField(rect, keycode, new GUIContent());
                EditorGUI.EndDisabledGroup();

            };
            keyboardInputList.onAddCallback = (list) =>
            {
                keyboardInputs.arraySize++;
                list.index = keyboardInputs.arraySize - 1;
            };
            keyboardInputList.onRemoveCallback = (list) =>
            {
                keyboardInputs.DeleteArrayElementAtIndex(list.index);
                if (list.index >= keyboardInputs.arraySize)
                {
                    list.index = keyboardInputs.arraySize - 1;
                }
            };
            keyboardInputList.onCanRemoveCallback = (list) =>
            {
                return keyboardInputs.arraySize > 0 ? true : false;
            };            

            mouseGroupList = new ReorderableList(serializedObject, mouseGroups, true, false, false, false);
            mouseInputList = new ReorderableList(serializedObject, mouseInputs, true, false, false, false);

            mouseGroupList.elementHeight = lineSpace;
            mouseGroupList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = mouseGroups.GetArrayElementAtIndex(index).FindPropertyRelative(MouseInputGroup.InputGroupName_Prop_Name);
                rect.height = lineHeight;
                rect.y += 3;
                EditorGUI.BeginDisabledGroup(isLock);
                EditorGUI.PropertyField(rect, element, new GUIContent());
                EditorGUI.EndDisabledGroup();
            };
            mouseGroupList.onAddCallback = (list) =>
            {
                mouseGroups.arraySize++;
                list.index = mouseGroups.arraySize - 1;
                var element = mouseGroups.GetArrayElementAtIndex(list.index).FindPropertyRelative(MouseInputGroup.InputGroupName_Prop_Name);
                element.stringValue = String.Format("New ({0})", list.index + 1);
            };
            mouseGroupList.onRemoveCallback = (list) =>
            {
                mouseGroups.DeleteArrayElementAtIndex(list.index);
                if (list.index >= mouseGroups.arraySize)
                {
                    list.index = mouseGroups.arraySize - 1;
                    mouseInputs = mouseGroups.GetArrayElementAtIndex(list.index).FindPropertyRelative(MouseInputGroup.Inputs_Prop_Name);
                    mouseInputList.serializedProperty = mouseInputs;
                }
            };
            mouseGroupList.onCanRemoveCallback = (list) =>
            {
                return mouseGroups.arraySize < 2 ? false : true;
            };
            mouseGroupList.onSelectCallback = (list) =>
            {
                mouseInputs = mouseGroups.GetArrayElementAtIndex(list.index).FindPropertyRelative(MouseInputGroup.Inputs_Prop_Name);
                mouseInputList.serializedProperty = mouseInputs;
            };

            mouseInputList.elementHeight = lineSpace;
            mouseInputList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var command = mouseInputs.GetArrayElementAtIndex(index).FindPropertyRelative(MouseInput.InputName_Prop_Name);
                var mousetype = mouseInputs.GetArrayElementAtIndex(index).FindPropertyRelative(MouseInput.InputMouseType_Prop_Value);
                var mousecode = mouseInputs.GetArrayElementAtIndex(index).FindPropertyRelative(MouseInput.InputMouseName_Prop_Value);
                rect.height = lineHeight;
                rect.width = 90;
                rect.y += 3;
                EditorGUI.BeginDisabledGroup(isLock);
                EditorGUI.PropertyField(rect, command, new GUIContent());
                rect.x += 95;
                EditorGUI.PropertyField(rect, mousetype, new GUIContent());
                rect.x += 95;
                EditorGUI.PropertyField(rect, mousecode, new GUIContent());
                EditorGUI.EndDisabledGroup();

            };
            mouseInputList.onAddCallback = (list) =>
            {
                mouseInputs.arraySize++;
                list.index = mouseInputs.arraySize - 1;
            };
            mouseInputList.onRemoveCallback = (list) =>
            {
                mouseInputs.DeleteArrayElementAtIndex(list.index);
                if (list.index >= mouseInputs.arraySize)
                {
                    list.index = mouseInputs.arraySize - 1;
                }
            };
            mouseInputList.onCanRemoveCallback = (list) =>
            {
                return mouseInputs.arraySize > 0 ? true : false;
            };
            
        }

        private void initializeAttribute()
        {
            scrollPos = new Vector2[4];
            unassignedKeyColor = Color.grey;
            assignedKeyColor = Color.cyan;
            isLock = false;
            isLockText = "Lock";

            updateAttribute();
        }


        private void updateAttribute()
        {
            keyCodeInputSetting = new Dictionary<KeyCode, List<string>>();
            foreach (KeyboardInputGroup keyboardInputGroup in inputSetting.KeyboardInputSetting)
            {
                foreach (KeyboardInput keyboardInput in keyboardInputGroup.Inputs)
                {
                    KeyCode inputKeyName = (KeyCode)Enum.Parse(typeof(KeyCode), keyboardInput.inputKeyName.ToString());
                    string inputKeyInfo = keyboardInputGroup.InputGroupName + " / " + keyboardInput.InputName;
                    if (!keyCodeInputSetting.ContainsKey(inputKeyName))
                    {
                        keyCodeInputSetting.Add(inputKeyName, new List<string>() { inputKeyInfo });
                    }
                    else
                    {
                        keyCodeInputSetting[inputKeyName].Add(inputKeyInfo);
                    }
                }
            }
        }

        public void SetInspectorLock()
        {
            keyboardGroupList.draggable = isLock;
            keyboardInputList.draggable = isLock;
            mouseGroupList.draggable = isLock;
            mouseInputList.draggable = isLock;
            isLock = !isLock;
            isLockText = isLockText == "Lock" ? "UnLock" : "Lock";
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            GUILayout.Space(5);
            if (GUILayout.Button(isLockText))
            {
                SetInspectorLock();
            }
            GUILayout.Space(5);
            EditorGUI.BeginDisabledGroup(isLock);
            inputSetting.InputSettingName = EditorGUILayout.TextField("Input Setting Name", inputSetting.InputSettingName);
            EditorGUI.EndDisabledGroup();

            // Keyboard Input Setting
            GUILayout.Space(5);
            GUILayout.Label("1. Keyboard Input Setting", EditorStyles.boldLabel);
            GUILayout.Space(5);
            drawKeyboard();

            serializedObject.Update();
            drawReorableList(0, 1, keyboardGroupList, keyboardInputList);
            serializedObject.ApplyModifiedProperties();

            // 2. Mouse Input Setting
            GUILayout.Space(5);
            GUILayout.Label("2. Mouse Input Setting", EditorStyles.boldLabel);
            GUILayout.Space(5);
            drawMouse();

            serializedObject.Update();
            drawReorableList(2, 3, mouseGroupList, mouseInputList);
            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                updateAttribute();
            }
        }


        private void drawReorableList(int a, int b, ReorderableList groupList, ReorderableList inputList)
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical("Box", GUILayout.Height(categoryHeight), GUILayout.Width(categoryWidth));
            GUILayout.Label("Category");
            scrollPos[a] = EditorGUILayout.BeginScrollView(scrollPos[a], GUILayout.Height(categoryHeight));
            groupList.DoLayoutList();
            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.Space(5);
            GUILayout.BeginVertical("Box", GUILayout.Height(categoryHeight));
            GUILayout.BeginHorizontal();
            GUILayout.Label("Command");
            GUILayout.Label("Type");
            GUILayout.Label("Code");
            GUILayout.EndHorizontal();
            scrollPos[b] = EditorGUILayout.BeginScrollView(scrollPos[b], GUILayout.Height(categoryHeight));
            inputList.DoLayoutList();
            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(120);
            EditorGUI.BeginDisabledGroup(isLock);
            if (GUILayout.Button("+", GUILayout.Width(lineHeight))) { groupList.onAddCallback(groupList); }
            EditorGUI.BeginDisabledGroup(!groupList.onCanRemoveCallback(groupList));
            if (GUILayout.Button("-", GUILayout.Width(lineHeight))) { groupList.onRemoveCallback(groupList); }
            EditorGUI.EndDisabledGroup();
            GUILayout.Space(315);
            if (GUILayout.Button("+", GUILayout.Width(lineHeight))) { inputList.onAddCallback(inputList); }
            EditorGUI.BeginDisabledGroup(!inputList.onCanRemoveCallback(inputList));
            if (GUILayout.Button("-", GUILayout.Width(lineHeight))) { inputList.onRemoveCallback(inputList); }
            EditorGUI.EndDisabledGroup();
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();
        }

        private void drawKeyboard()
        {
            GUI.skin.box.alignment = TextAnchor.MiddleCenter;
            GUILayout.BeginHorizontal();
            GUI.backgroundColor = unassignedKeyColor;
            GUILayout.Box("", GUILayout.Width(lineHeight), GUILayout.Height(lineHeight));
            GUI.backgroundColor = Color.white;
            GUILayout.Label("Unassigned Key");
            GUILayout.Space(20);
            GUI.backgroundColor = assignedKeyColor;
            GUILayout.Box("", GUILayout.Width(lineHeight), GUILayout.Height(lineHeight));
            GUI.backgroundColor = Color.white;
            GUILayout.Label("Assigned Key");
            GUILayout.EndHorizontal();

            for (int i = 0; i < keyboardName.Length; i++)
            {
                GUILayout.BeginHorizontal();
                for (int j = 0; j < keyboardName[i].Length; j++)
                {
                    GUI.backgroundColor = unassignedKeyColor;
                    string tooltip = defaultToolTip;
                    if (keyCodeInputSetting.ContainsKey(keyboardCode[i][j]))
                    {
                        tooltip = tooltipString(keyCodeInputSetting[keyboardCode[i][j]]);
                        GUI.backgroundColor = assignedKeyColor;
                    }
                    var content = new GUIContent(keyboardName[i][j], tooltip);

                    GUILayout.Box(content, GUILayout.Width(keyboardCellSize * keyboardSize[i][j]), GUILayout.Height(keyboardCellSize));
                }
                GUI.backgroundColor = Color.white;
                GUILayout.EndHorizontal();
            }
        }

        private string tooltipString(List<string> info)
        {
            if (info == null || info.Count == 0)
            {
                return defaultToolTip;
            }
            string str = "";
            for (int i = 0; i < info.Count; i++)
            {
                str += info[i].ToString();
                if (i != info.Count - 1) { str += "\n"; }
            }
            return str;
        }

        private void drawMouse()
        {

        }


        /*
        private void initializeAttribute()
        {
            selectedSettingGroupIndex = new int[2] { 0, 0 };
            selectedSettingInputsIndex = new int[2] { 0, 0 };
            unassignedKeyColor = Color.grey;
            assignedKeyColor = Color.cyan;

            updateAttribute();
        }

        private void updateAttribute()
        {
            keyCodeInputSetting = new Dictionary<KeyCode, List<string>>();
            foreach (KeyboardInputGroup keyboardInputGroup in inputSetting.KeyboardInputSetting)
            {
                foreach (KeyboardInput keyboardInput in keyboardInputGroup.Inputs)
                {
                    KeyCode inputKeyName = (KeyCode)Enum.Parse(typeof(KeyCode), keyboardInput.inputKeyName.ToString());
                    string inputKeyInfo = keyboardInputGroup.InputGroupName + " / " + keyboardInput.InputName;
                    if (!keyCodeInputSetting.ContainsKey(inputKeyName))
                    {
                        keyCodeInputSetting.Add(inputKeyName, new List<string>() { inputKeyInfo });
                    }
                    else
                    {
                        keyCodeInputSetting[inputKeyName].Add(inputKeyInfo);
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Space(5);
            inputSetting.InputSettingName = EditorGUILayout.TextField("InputSetting Name", inputSetting.InputSettingName);
            GUILayout.Space(5);

            drawKeyboardInputSetting();

            GUILayout.Space(5);
            drawMouseInputSetting();

            GUILayout.Space(10);

            if (GUI.changed)
            {
                updateAttribute();
            }
        }

        private void drawKeyboard()
        {
            GUI.skin.box.alignment = TextAnchor.MiddleCenter;
            GUILayout.BeginHorizontal();
            GUI.backgroundColor = unassignedKeyColor;
            GUILayout.Box("", GUILayout.Width(keyboardCellSize * 0.7f), GUILayout.Height(keyboardCellSize * 0.7f));
            GUI.backgroundColor = Color.white;
            GUILayout.Label("Unassigned Key");
            GUILayout.Space(20);
            GUI.backgroundColor = assignedKeyColor;
            GUILayout.Box("", GUILayout.Width(keyboardCellSize * 0.7f), GUILayout.Height(keyboardCellSize * 0.7f));
            GUI.backgroundColor = Color.white;
            GUILayout.Label("Assigned Key");
            GUILayout.EndHorizontal();

            for (int i = 0; i < keyboardName.Length; i++)
            {
                GUILayout.BeginHorizontal();
                for (int j = 0; j < keyboardName[i].Length; j++)
                {
                    GUI.backgroundColor = unassignedKeyColor;
                    string tooltip = defaultToolTip;
                    if (keyCodeInputSetting.ContainsKey(keyboardCode[i][j]))
                    {
                        tooltip = tooltipString(keyCodeInputSetting[keyboardCode[i][j]]);
                        GUI.backgroundColor = assignedKeyColor;
                    }
                    var content = new GUIContent(keyboardName[i][j], tooltip);

                    GUILayout.Box(content, GUILayout.Width(keyboardCellSize * keyboardSize[i][j]), GUILayout.Height(keyboardCellSize));
                }
                GUI.backgroundColor = Color.white;
                GUILayout.EndHorizontal();
            }
        }

        private string tooltipString(List<string> info)
        {
            if (info == null || info.Count == 0)
            {
                return defaultToolTip;
            }
            string str = "";
            for (int i = 0; i < info.Count; i++)
            {
                str += info[i].ToString();
                if (i != info.Count - 1) { str += "\n"; }
            }
            return str;
        }

        private void drawKeyboardInputSetting()
        {
            GUILayout.Label("1. Keyboard InputSetting", EditorStyles.boldLabel);
            GUILayout.Space(5);
            drawKeyboard();
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            drawInputGroup<KeyboardInputGroup, KeyboardInput>(inputSetting.KeyboardInputSetting);
            drawKeyboardInput();
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            setInputGroupName<KeyboardInputGroup, KeyboardInput>(inputSetting.KeyboardInputSetting);
            AddInputGroupName<KeyboardInputGroup, KeyboardInput>(inputSetting.KeyboardInputSetting);
            RemoveInputGroupName<KeyboardInputGroup, KeyboardInput>(inputSetting.KeyboardInputSetting);
            GUILayout.Space(300);
            AddInputs<KeyboardInputGroup, KeyboardInput>(inputSetting.KeyboardInputSetting);
            RemoveInputs<KeyboardInputGroup, KeyboardInput>(inputSetting.KeyboardInputSetting);
            GUILayout.EndHorizontal();
        }

        private void drawMouse()
        {
            GUILayout.Label("Mouse �߰�");
        }

        private void drawMouseInputSetting()
        {
            GUILayout.Label("2. Mouse InputSetting", EditorStyles.boldLabel);
            GUILayout.Space(5);
            drawMouse();
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            drawInputGroup<MouseInputGroup, MouseInput>(inputSetting.MouseInputSetting);
            drawMouseInput();
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            setInputGroupName<MouseInputGroup, MouseInput>(inputSetting.MouseInputSetting);
            AddInputGroupName<MouseInputGroup, MouseInput>(inputSetting.MouseInputSetting);
            RemoveInputGroupName<MouseInputGroup, MouseInput>(inputSetting.MouseInputSetting);
            GUILayout.Space(300);
            AddInputs<MouseInputGroup, MouseInput>(inputSetting.MouseInputSetting);
            RemoveInputs<MouseInputGroup, MouseInput>(inputSetting.MouseInputSetting);
            GUILayout.EndHorizontal();
        }

        private int returnGroupNumber<T, U>(List<T> inputGroup) where T : PlayerInputGroup<U> where U : PlayerInput, new()
        {
            int groupNumber;
            if (new U() is KeyboardInput) { groupNumber = 0; }
            else { groupNumber = 1; }
            return groupNumber;
        }

        private void drawInputGroup<T, U>(List<T> inputGroup) where T : PlayerInputGroup<U> where U : PlayerInput, new()
        {
            GUILayout.BeginVertical("Box", GUILayout.Height(categoryHeight), GUILayout.Width(categoryWidth));
            GUILayout.Label("Category");
            if (inputGroup.Count == 0)
            {
                GUILayout.Space(40);
                GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                GUILayout.Label("No Catergory");
                GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            }
            else
            {
                int groupNumber = returnGroupNumber<T, U>(inputGroup);
                string[] groupNames = new string[inputGroup.Count];
                for (int i = 0; i < inputGroup.Count; i++)
                {
                    groupNames[i] = inputGroup[i].InputGroupName;
                }

                EditorGUI.BeginChangeCheck();
                selectedSettingGroupIndex[groupNumber] = GUILayout.SelectionGrid(selectedSettingGroupIndex[groupNumber], groupNames, 1);
                if (EditorGUI.EndChangeCheck())
                {
                    GUI.FocusControl(null);
                }
            }
            GUILayout.EndVertical();
        }

        private void setInputGroupName<T, U>(List<T> inputGroup) where T : PlayerInputGroup<U> where U : PlayerInput, new()
        {
            if (inputGroup.Count == 0)
            {
                GUILayout.Space(105);
            }
            else
            {
                int groupNumber = returnGroupNumber<T, U>(inputGroup);
                int groupIndex = selectedSettingGroupIndex[groupNumber];
                inputGroup[groupIndex].InputGroupName = GUILayout.TextField(inputGroup[groupIndex].InputGroupName, GUILayout.Width(105));
            }
        }

        private void AddInputGroupName<T, U>(List<T> inputGroup) where T : PlayerInputGroup<U>, new() where U : PlayerInput
        {
            if (GUILayout.Button("+", GUILayout.Width(keyboardCellSize * 0.7f)))
            {
                inputGroup.Add(new T());
            }
        }
        private void RemoveInputGroupName<T, U>(List<T> inputGroup) where T : PlayerInputGroup<U> where U : PlayerInput, new()
        {

            EditorGUI.BeginDisabledGroup(inputGroup.Count == 0);
            if (GUILayout.Button("-", GUILayout.Width(keyboardCellSize * 0.7f)))
            {
                int groupNumber = returnGroupNumber<T, U>(inputGroup);
                int groupIndex = selectedSettingGroupIndex[groupNumber];
                inputGroup.Remove(inputGroup[groupIndex]);
                if (selectedSettingGroupIndex[groupNumber] >= inputGroup.Count) { selectedSettingGroupIndex[groupNumber] = inputGroup.Count - 1; }
                if (selectedSettingGroupIndex[groupNumber] < 0) { selectedSettingGroupIndex[groupNumber] = 0; }
            }
            EditorGUI.EndDisabledGroup();
        }

        private void AddInputs<T, U>(List<T> inputGroup) where T : PlayerInputGroup<U> where U : PlayerInput, new()
        {
            if (inputGroup.Count != 0 && GUILayout.Button("+", GUILayout.Width(keyboardCellSize * 0.7f)))
            {
                int groupNumber = returnGroupNumber<T, U>(inputGroup);
                int groupIndex = selectedSettingGroupIndex[groupNumber];
                inputGroup[groupIndex].Inputs.Add(new U());
            }
        }

        private void RemoveInputs<T, U>(List<T> inputGroup) where T : PlayerInputGroup<U> where U : PlayerInput, new()
        {

            EditorGUI.BeginDisabledGroup(inputGroup.Count == 0 || inputGroup[selectedSettingGroupIndex[returnGroupNumber<T, U>(inputGroup)]].Inputs.Count == 0);
            if (inputGroup.Count != 0 && GUILayout.Button("-", GUILayout.Width(keyboardCellSize * 0.7f)))
            {
                int groupNumber = returnGroupNumber<T, U>(inputGroup);
                int groupIndex = selectedSettingGroupIndex[groupNumber];
                int inputIndex = selectedSettingInputsIndex[groupNumber];
                inputGroup[groupIndex].Inputs.Remove(inputGroup[groupIndex].Inputs[inputIndex]);
                if (inputIndex >= inputGroup[groupIndex].Inputs.Count) { selectedSettingInputsIndex[groupNumber] = 0; }
                if (inputIndex < 0) { selectedSettingInputsIndex[groupNumber] = 0; }
            }
            EditorGUI.EndDisabledGroup();
        }


        private void drawInputLabel()
        {
            GUILayout.BeginVertical("Box", GUILayout.Height(categoryHeight));

            GUILayout.BeginHorizontal();
            GUILayout.Label("Command", GUILayout.Width(100));
            GUILayout.Space(30);
            GUILayout.Label("KeyType", GUILayout.Width(90));
            GUILayout.Space(30);
            GUILayout.Label("KeyCode", GUILayout.Width(70));
            GUILayout.EndHorizontal();
        }

        private void drawKeyboardInput()
        {
            drawInputLabel();

            if (inputSetting.KeyboardInputSetting.Count == 0 || inputSetting.KeyboardInputSetting[selectedSettingGroupIndex[0]].Inputs.Count == 0)
            {
                GUILayout.Space(40);
                GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                GUILayout.Label("No Keyboard Input Setting");
                GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            }
            else
            {
                foreach (KeyboardInput keyboard in inputSetting.KeyboardInputSetting[selectedSettingGroupIndex[0]].Inputs)
                {
                    GUILayout.BeginHorizontal();
                    keyboard.InputName = GUILayout.TextField(keyboard.InputName, GUILayout.Width(100));
                    GUILayout.Space(30);
                    keyboard.inputKeyType = (KeyboardInputType)EditorGUILayout.EnumPopup(keyboard.inputKeyType, GUILayout.Width(90));
                    GUILayout.Space(30);
                    keyboard.inputKeyName = (KeyboardInputName)EditorGUILayout.EnumPopup(keyboard.inputKeyName, GUILayout.Width(70));
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
        }

        private void drawMouseInput()
        {
            drawInputLabel();

            if (inputSetting.MouseInputSetting.Count == 0 || inputSetting.MouseInputSetting[selectedSettingGroupIndex[1]].Inputs.Count == 0)
            {
                GUILayout.Space(40);
                GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                GUILayout.Label("No Mouse Input Setting");
                GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            }
            else
            {
                foreach (MouseInput mouse in inputSetting.MouseInputSetting[selectedSettingGroupIndex[1]].Inputs)
                {
                    GUILayout.BeginHorizontal();
                    mouse.InputName = GUILayout.TextField(mouse.InputName, GUILayout.Width(100));
                    GUILayout.Space(30);
                    mouse.inputMouseType = (MouseInputType)EditorGUILayout.EnumPopup(mouse.inputMouseType, GUILayout.Width(90));
                    GUILayout.Space(30);
                    mouse.inputMouseName = (MouseInputName)EditorGUILayout.EnumPopup(mouse.inputMouseName, GUILayout.Width(70));
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
        }*/
    }
}