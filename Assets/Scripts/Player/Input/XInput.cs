using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XPlayer.Input.InputSetting;
using XPlayer.Input.Keyboard;
using XPlayer.Input.Mouse;

#if UNITY_EDITOR 
using UnityEditor;
#endif

namespace XPlayer.Input.InputManager
{
    public class XInput : ScriptableObject
    {
        private const string SettingFileDirectory = "Assets/Resources";
        private const string SettingFilePath = "Assets/Resources/XInput.asset";

        // lazy initialize singleton
        private static XInput _instance;
        public static XInput Instance
        {
            get
            {
                if (_instance != null)
                {
                    if(_instance.PlayerInputSettings == null)
                    {
                        _instance.PlayerInputSettings = new List<InputSetting.InputSetting>();
                    }
                    _instance.PresentIndex = 0;
                    return _instance;
                }
                // if _instance is null and UISetting.asset exist in Resources folder, you get UISetting
                _instance = Resources.Load<XInput>("XInput");

                // but, UISetting.asset has never been made, make it the first time you approach it automatically
                // UISetting is used in Project(Editortime), not Runtime
#if UNITY_EDITOR
                if (_instance == null)
                {
                    // AssetDatabase is in UnityEditor and it can manage Unity Project Assets
                    // use AssetDatabase to create a meta file immediately
                    if (!AssetDatabase.IsValidFolder(SettingFileDirectory))
                    {
                        AssetDatabase.CreateFolder("Assets", "Resources");
                    }
                    // unexpected error
                    _instance = AssetDatabase.LoadAssetAtPath<XInput>(SettingFilePath);
                    // if file doesnt exist in SettingFilePath
                    if (_instance == null)
                    {
                        _instance = CreateInstance<XInput>();
                        /*_instance.PlayerInputSetting = ScriptableObject.CreateInstance<InputSetting.InputSetting>();
                        AssetDatabase.AddObjectToAsset(_instance.PlayerInputSetting, SettingFilePath);*/
                        AssetDatabase.CreateAsset(_instance, SettingFilePath);
                        AssetDatabase.ImportAsset(SettingFilePath);
                        _instance.PlayerInputSettings = new List<InputSetting.InputSetting>();
                        _instance.PresentIndex = 0;
                    }
                }
#endif

                return _instance;
            }
        }

        public List<InputSetting.InputSetting> PlayerInputSettings;
        public int PresentIndex;

        public InputSetting.InputSetting this[int index]
        {
            get
            {
                if(index < 0 || index >= PlayerInputSettings.Count) { return null; }
                else { return PlayerInputSettings[index]; }
            }
        }
        
        public void SetInputSetting(int index)
        {
            PresentIndex = index;
            this[index].InputUnLockAll();
        }

        public void SetInputSetting(string name)
        {
            bool isValid = false;
            for (int i = 0; i < PlayerInputSettings.Count; i++)
            {
                if (PlayerInputSettings[i].InputSettingName.Equals(name))
                {
                    PresentIndex = i;
                    isValid = true;
                    break;
                }
            }
            if(isValid) { this[PresentIndex].InputUnLockAll(); }
            else { Debug.LogError($"Input Setting with name {name} doesn't exist."); }
        }

        
        public KeyboardInput GetKey(string name)
        {
            return this[PresentIndex].GetInput<KeyboardInputGroup, KeyboardInput>(name, this[PresentIndex].KeyboardInputSetting);
        }

        public MouseInput GetMouse(string name)
        {
            return this[PresentIndex].GetInput<MouseInputGroup, MouseInput>(name, this[PresentIndex].MouseInputSetting);
        }

        public KeyboardInputGroup GetKeyGroup(string name)
        {
            return this[PresentIndex].GetInputGroup<KeyboardInputGroup, KeyboardInput>(name, this[PresentIndex].KeyboardInputSetting);
        }

        public MouseInputGroup GetMouseGroup(string name)
        {
            return this[PresentIndex].GetInputGroup<MouseInputGroup, MouseInput>(name, this[PresentIndex].MouseInputSetting);
        }

        public bool KeyInput(string name)
        {
            if (GetKey(name) == null) return false;
            return GetKey(name).IsInput;
        }

        public bool KeyInput(string group, string key)
        {
            if (GetKeyGroup(group) == null || GetKeyGroup(group).GetInput(name) == null) return false;
            return GetKeyGroup(group).GetInput(key).IsInput;
        }

        public bool MouseInput(string name)
        {
            if (GetMouse(name) == null) return false;
            return GetMouse(name).IsInput;
        }

        public bool MouseInput(string group, string mouse)
        {
            if (GetMouseGroup(group) == null || GetMouseGroup(group).GetInput(name) == null) return false;
            return GetMouseGroup(group).GetInput(mouse).IsInput;
        }

        public void InputLockAll()
        {
            this[PresentIndex].InputLockAll();
        }

        public void KeyInputLockAll(string name = null)
        {
            if (name == null)
            {
                this[PresentIndex].InputLockAll<KeyboardInputGroup, KeyboardInput>(this[PresentIndex].KeyboardInputSetting);
            }
            else
            {
                this[PresentIndex].InputLockAll<KeyboardInputGroup, KeyboardInput>(name, this[PresentIndex].KeyboardInputSetting);
            }
        }

        public void MouseInputLockAll(string name = null)
        {
            if (name == null)
            {
                this[PresentIndex].InputLockAll<MouseInputGroup, MouseInput>(this[PresentIndex].MouseInputSetting);
            }
            else
            {
                this[PresentIndex].InputLockAll<MouseInputGroup, MouseInput>(name, this[PresentIndex].MouseInputSetting);
            }
        }

        public void InputUnLockAll()
        {
            this[PresentIndex].InputUnLockAll();
        }

        public void KeyInputUnLockAll(string name = null)
        {
            if (name == null)
            {
                this[PresentIndex].InputUnLockAll<KeyboardInputGroup, KeyboardInput>(this[PresentIndex].KeyboardInputSetting);
            }
            else
            {
                this[PresentIndex].InputUnLockAll<KeyboardInputGroup, KeyboardInput>(name, this[PresentIndex].KeyboardInputSetting);
            }
        }

        public void MouseInputUnLockAll(string name = null)
        {
            if (name == null)
            {
                this[PresentIndex].InputUnLockAll<MouseInputGroup, MouseInput>(this[PresentIndex].MouseInputSetting);
            }
            else
            {
                this[PresentIndex].InputUnLockAll<MouseInputGroup, MouseInput>(name, this[PresentIndex].MouseInputSetting);
            }
        }

        public void KeyInputUnLockOnly(string groupName, string inputName)
        {
            this[PresentIndex].InputUnLockOnly<KeyboardInputGroup, KeyboardInput>(groupName, inputName, this[PresentIndex].KeyboardInputSetting);
        }

        public void KeyInputUnLockOnly(string name, bool isGroupName = true)
        {
            this[PresentIndex].InputUnLockOnly<KeyboardInputGroup, KeyboardInput>(name, this[PresentIndex].KeyboardInputSetting, isGroupName);
        }

        public void MouseInputUnLockOnly(string groupName, string inputName)
        {
            this[PresentIndex].InputUnLockOnly<MouseInputGroup, MouseInput>(groupName, inputName, this[PresentIndex].MouseInputSetting);
        }

        public void MouseInputUnLockOnly(string name, bool isGroupName = true)
        {
            this[PresentIndex].InputUnLockOnly<MouseInputGroup, MouseInput>(name, this[PresentIndex].MouseInputSetting, isGroupName);
        }
        public void KeyInputLockOnly(string groupName, string inputName)
        {
            this[PresentIndex].InputLockOnly<KeyboardInputGroup, KeyboardInput>(groupName, inputName, this[PresentIndex].KeyboardInputSetting);
        }

        public void KeyInputLockOnly(string name, bool isGroupName = true)
        {
            this[PresentIndex].InputLockOnly<KeyboardInputGroup, KeyboardInput>(name, this[PresentIndex].KeyboardInputSetting, isGroupName);
        }

        public void MouseInputLockOnly(string groupName, string inputName)
        {
            this[PresentIndex].InputLockOnly<MouseInputGroup, MouseInput>(groupName, inputName, this[PresentIndex].MouseInputSetting);
        }

        public void MouseInputLockOnly(string name, bool isGroupName = true)
        {
            this[PresentIndex].InputLockOnly<MouseInputGroup, MouseInput>(name, this[PresentIndex].MouseInputSetting, isGroupName);
        }
    }
}
