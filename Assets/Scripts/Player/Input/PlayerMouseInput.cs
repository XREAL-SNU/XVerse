using System;
using UnityEngine;
using System.Collections.Generic;

namespace XPlayer.Input.Mouse
{
    public enum MouseInputName
    {
        Left, Right, Wheel
    }

    public enum MouseInputType
    {
        MouseDown, Mouse, MouseUp
    }

    [Serializable]
    public class MouseInput : PlayerInput
    {
        public MouseInputName inputMouseName;
        public MouseInputType inputMouseType;
        public static string InputMouseType_Prop_Value => nameof(inputMouseName);
        public static string InputMouseName_Prop_Value => nameof(inputMouseType);

        public bool IsInput
        {
            get
            {
                switch (inputMouseType)
                {
                    case MouseInputType.Mouse:
                        return isActiveInput && UnityEngine.Input.GetMouseButton((int)inputMouseName);
                    case MouseInputType.MouseUp:
                        return isActiveInput && UnityEngine.Input.GetMouseButtonUp((int)inputMouseName);
                    case MouseInputType.MouseDown:
                        return isActiveInput && UnityEngine.Input.GetMouseButtonDown((int)inputMouseName);
                    default:
                        Debug.LogError("MouseInputType Error");
                        return false;
                }
            }
        }

        public MouseInput(string name, MouseInputName mouseName, MouseInputType mouseType)
        {
            InputName = name;
            isActiveInput = true;
            inputMouseName = mouseName;
            inputMouseType = mouseType;
        }

        public MouseInput()
        {
            isActiveInput = true;
        }

        public void SetMouseInputName(MouseInputName keyName)
        {
            if (Enum.IsDefined(typeof(KeyCode), inputMouseName.ToString()))
            {
                inputMouseName = keyName;
            }
        }

        public void CopyFrom(MouseInput mouse)
        {
            if (mouse == null) { return; }
            InputName = mouse.InputName;
            inputMouseName = mouse.inputMouseName;
            inputMouseType = mouse.inputMouseType;
        }

    }

    [Serializable]
    public class MouseInputGroup : PlayerInputGroup<MouseInput>
    {
        public MouseInputGroup(string name)
        {
            InputGroupName = name;
        }

        public MouseInputGroup(string name, List<MouseInput> mouseSet) : this(name)
        {
            Inputs = mouseSet;
        }

        public MouseInputGroup()
        {
            InputGroupName = "New (1)";
            Inputs = new List<MouseInput>();
        }

        public void CopyFrom(MouseInputGroup mouseGroup)
        {
            if (mouseGroup == null) { return; }
            foreach (MouseInput mouse in mouseGroup.Inputs)
            {
                MouseInput input = new MouseInput();
                input.CopyFrom(mouse);
                Inputs.Add(input);
            }
            InputGroupName = mouseGroup.InputGroupName;
        }

    }
}
