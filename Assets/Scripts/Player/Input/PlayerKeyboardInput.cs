using System;
using UnityEngine;
using System.Collections.Generic;

namespace XPlayer.Input.Keyboard
{
    public enum KeyboardInputName
    {
        W, A, S, D, T, Alpha1, Alpha2, Alpha3, Alpha4, G, Space, U
    };

    public enum KeyboardInputType
    {
        KeyDown, Key, KeyUp
    }

    [Serializable]
    public class KeyboardInput : PlayerInput
    {
        public KeyboardInputName inputKeyName;
        public KeyboardInputType inputKeyType;

        public bool IsInput
        {
            get
            {
                switch (inputKeyType)
                {
                    case KeyboardInputType.Key:
                        return isActiveInput && UnityEngine.Input.GetKey(ToKeyCode());
                    case KeyboardInputType.KeyDown:
                        return isActiveInput && UnityEngine.Input.GetKeyDown(ToKeyCode());
                    case KeyboardInputType.KeyUp:
                        return isActiveInput && UnityEngine.Input.GetKeyUp(ToKeyCode());
                    default:
                        Debug.LogError("KeyboardInputType Error");
                        return false;
                }
            }
        }

        public KeyboardInput(string name, KeyboardInputName keyName, KeyboardInputType keyType)
        {
            InputName = name;
            isActiveInput = true;
            if (Enum.IsDefined(typeof(KeyCode), inputKeyName.ToString()))
            {
                inputKeyName = keyName;
                inputKeyType = keyType;
            }
        }

        public KeyboardInput()
        {
            isActiveInput = true;
        }

        public KeyCode ToKeyCode()
        {
            return (KeyCode)Enum.Parse(typeof(KeyCode), inputKeyName.ToString());
        }

        public void SetKeyboardInputName(KeyboardInputName keyName)
        {
            if (Enum.IsDefined(typeof(KeyCode), inputKeyName.ToString()))
            {
                inputKeyName = keyName;
            }
        }

        public void CopyFrom(KeyboardInput keyboard)
        {
            if (keyboard == null) { return; }
            InputName = keyboard.InputName;
            inputKeyName = keyboard.inputKeyName;
            inputKeyType = keyboard.inputKeyType;
        }
    }

    [Serializable]
    public class KeyboardInputGroup : PlayerInputGroup<KeyboardInput>
    {
        public KeyboardInputGroup(string name)
        {
            InputGroupName = name;
        }

        public KeyboardInputGroup(string name, List<KeyboardInput> keySet) : this(name)
        {
            Inputs = keySet;
        }

        public KeyboardInputGroup()
        {
            Inputs = new List<KeyboardInput>();
        }

        public void CopyFrom(KeyboardInputGroup keyboardGroup)
        {
            if (keyboardGroup == null) { return; }
            foreach(KeyboardInput keyboard in keyboardGroup.Inputs)
            {
                KeyboardInput input = new KeyboardInput();
                input.CopyFrom(keyboard);
                Inputs.Add(input);
            }
            InputGroupName = keyboardGroup.InputGroupName;
        }
    }

}